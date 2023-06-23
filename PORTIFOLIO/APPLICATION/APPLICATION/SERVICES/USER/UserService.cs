using APPLICATION.APPLICATION.CONFIGURATIONS;
using APPLICATION.DOMAIN.CONTRACTS.FACADE;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.TOKEN;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.SERVICEBUS.MESSAGE;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER.ROLE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.EXCEPTIONS.USER;
using APPLICATION.DOMAIN.UTILS.Extensions;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.DOMAIN.VALIDATORS;
using APPLICATION.INFRAESTRUTURE.JOBS.QUEUED;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Net;
using System.Security.Claims;
using System.Web;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace APPLICATION.APPLICATION.SERVICES.USER
{
    /// <summary>
    /// Serviço de usuários.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<RoleEntity> _roleManager;
        private readonly IOptions<AppSettings> _appsettings;
        private readonly ITokenService _tokenService;
        private readonly IUtilFacade _utilFacade;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="appsettings"></param>
        /// <param name="tokenService"></param>
        /// <param name="utilFacade"></param>
        public UserService(IUserRepository userRepository, RoleManager<RoleEntity> roleManager, IOptions<AppSettings> appsettings, ITokenService tokenService, IUtilFacade utilFacade)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _appsettings = appsettings;
            _tokenService = tokenService;
            _utilFacade = utilFacade;
        }

        /// <summary>
        /// Método responsável por fazer a authorização do usuário.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<ObjectResult> AuthenticationAsync(LoginRequest loginRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AuthenticationAsync)}\n");

            try
            {
                await new AuthenticationValidator().ValidateAsync(
                    loginRequest).ContinueWith(async (validationTask) =>
                    {
                        var validation = validationTask.Result;

                        if (validation.IsValid is false)
                            await validation.GetValidationErrors();
                    });

                return await _userRepository.GetWithUsernameAsync(
                    loginRequest.Username).ContinueWith(async (userEntityTask) =>
                    {
                        var userEntity =
                            userEntityTask.Result
                            ?? throw new NotFoundUserException(loginRequest);

                        await _userRepository.PasswordSignInAsync(
                            userEntity, loginRequest.Password, true, true).ContinueWith((signInResultTask) =>
                        {
                            var signInResult = signInResultTask.Result;

                            if (signInResult.Succeeded is false) ThrownAuthorizationException(signInResult, loginRequest);
                        });

                        var tokenJWT = await GenerateTokenJwtAsync(userEntity, loginRequest);

                        return new OkObjectResult(
                            new ApiResponse<TokenJWT>(
                                true, HttpStatusCode.Created, tokenJWT, new List<DadosNotificacao>  {
                                    new DadosNotificacao("Token criado com sucesso!")
                                })
                            );

                    }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsável por recuperar um usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<ObjectResult> GetByIdAsync(Guid userId)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetByIdAsync)}\n");

            try
            {
                return await _userRepository.GetByAsync(userId).ContinueWith(userEntityTask =>
                {
                    var userEntity = userEntityTask.Result;

                    if (userEntity is null) throw new NotFoundUserException(userId);

                    var userResponse = userEntity.ToResponse();

                    return new OkObjectResult(
                        new ApiResponse<UserResponse>(
                            true, HttpStatusCode.OK, userResponse, new List<DadosNotificacao> { new DadosNotificacao("Usuario recuperado com sucesso.") })
                        );
                });
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        ///  Método responsavel por criar um novo usuário.
        /// </summary>
        /// <param name="userCreateRequest"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public async Task<ObjectResult> CreateAsync(UserCreateRequest userCreateRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(CreateAsync)}\n");

            try
            {
                await new CreateUserValidator().ValidateAsync(
                    userCreateRequest).ContinueWith(async (validationTask) =>
                    {
                        var validation = validationTask.Result;

                        if (validation.IsValid is false) await validation.GetValidationErrors();
                    });

                var user = userCreateRequest.ToIdentityUser();

                return await _userRepository.CreateUserAsync(
                    user, userCreateRequest.Password).ContinueWith(identityResultTask =>
                    {
                        var identityResult = identityResultTask.Result;

                        if (identityResult.Succeeded is false) throw new CustomException(
                            HttpStatusCode.BadRequest, userCreateRequest, identityResult.Errors.Select((e) => new DadosNotificacao(e.Code.CustomExceptionMessage())).ToList());

                        //EnviarEmail().Wait();

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.Created, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário criado com sucesso.") }));
                    });
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsável por atualizar um usuário.
        /// </summary>
        /// <param name="userUpdateRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundUserException"></exception>
        /// <exception cref="CustomException"></exception>
        public async Task<ObjectResult> UpdateAsync(UserUpdateRequest userUpdateRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(UpdateAsync)}\n");

            try
            {
                await new UpdateUserValidator().ValidateAsync(
                  userUpdateRequest).ContinueWith(async (validationTask) =>
                  {
                      var validation = validationTask.Result;

                      if (validation.IsValid is false)
                          await validation.GetValidationErrors();
                  });

                return await _userRepository.GetByAsync(userUpdateRequest.Id).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                        userEntityTask.Result
                        ?? throw new NotFoundUserException(userUpdateRequest);

                    if (userUpdateRequest.UserName.Equals(userEntity.UserName) is false)
                    {
                        await _userRepository.SetUserNameAsync(
                           userEntity, userUpdateRequest.UserName).ContinueWith(identityResultTask =>
                           {
                               var identityResult = identityResultTask.Result;

                               if (identityResult.Succeeded is false)
                                   throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                       new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                           });
                    }

                    if (string.IsNullOrEmpty(userUpdateRequest.Password) is false)
                    {
                        await _userRepository.ChangePasswordAsync(
                            userEntity, userUpdateRequest.CurrentPassword, userUpdateRequest.Password).ContinueWith(identityResultTask =>
                            {
                                var identityResult = identityResultTask.Result;

                                if (identityResult.Succeeded is false)
                                    throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                        new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                            });
                    }

                    if (userUpdateRequest.Email.Equals(userEntity.Email) is false)
                    {
                        await _userRepository.SetEmailAsync(
                            userEntity, userUpdateRequest.Email).ContinueWith(identityResultTask =>
                            {
                                var identityResult = identityResultTask.Result;

                                if (identityResult.Succeeded is false)
                                    throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                        new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                            });

                        //await ConfirmeUserForEmailAsync(userEntity);
                    }

                    if (userUpdateRequest.PhoneNumber.Equals(userEntity.PhoneNumber) is false)
                    {
                        await _userRepository.SetPhoneNumberAsync(
                            userEntity, userUpdateRequest.PhoneNumber).ContinueWith(identityResultTask =>
                            {
                                var identityResult = identityResultTask.Result;

                                if (identityResult.Succeeded is false)
                                    throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                        new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                            });
                    }

                    await _userRepository.UpdateUserAsync(
                            userEntity.TransformUserEntityFromUserUpdateRequest(
                                userUpdateRequest));

                    return new OkObjectResult(
                       new ApiResponse<object>(
                           true, HttpStatusCode.OK, userEntity.ToResponse(),
                           new List<DadosNotificacao> { new DadosNotificacao("Usuário atualizado com sucesso.") }));

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsavel por ativar um novo usuário.
        /// </summary>
        /// <param name="activateUserRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundUserException"></exception>
        /// <exception cref="CustomException"></exception>
        public async Task<ObjectResult> ActivateUserAsync(ActivateUserRequest activateUserRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(ActivateUserAsync)}\n");

            try
            {
                return await _userRepository.GetByAsync(activateUserRequest.UserId).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                        userEntityTask.Result
                        ?? throw new NotFoundUserException(activateUserRequest);

                    await _userRepository.ConfirmEmailAsync(userEntity, HttpUtility.UrlDecode(
                        activateUserRequest.Code.Replace(";", "%"))).ContinueWith(identityResultTask =>
                        {
                            var identityResult = identityResultTask.Result;

                            if (identityResult.Succeeded is false)
                                throw new CustomException(HttpStatusCode.BadRequest, activateUserRequest,
                                    new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        });

                    return new ObjectResult(
                        new ApiResponse<object>(
                            true, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário ativado com sucesso!") }));

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsavel por criar uma nova claim para o usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="claimRequest"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<ObjectResult> AddUserClaimAsync(string username, ClaimRequest claimRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddUserClaimAsync)}\n");

            try
            {
                await new AddClaimValidator().ValidateAsync(
                  claimRequest).ContinueWith(async (validationTask) =>
                  {
                      var validation = validationTask.Result;

                      if (string.IsNullOrEmpty(username))
                          throw new CustomException(HttpStatusCode.BadRequest, null,
                             new List<DadosNotificacao> { new DadosNotificacao("Campo username deve ter um valor!") });

                      if (validation.IsValid is false)
                          await validation.GetValidationErrors();
                  });

                return await _userRepository.GetWithUsernameAsync(username).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                       userEntityTask.Result
                       ?? throw new NotFoundUserException(new
                       {
                           Username = username,
                           ClaimRequest = claimRequest
                       });

                    return await _userRepository.AddClaimUserAsync(userEntity, new Claim(claimRequest.Type, claimRequest.Value)).ContinueWith(identityResultTask =>
                    {
                        var identityResult = identityResultTask.Result;

                        if (identityResult.Succeeded is false)
                            throw new CustomException(HttpStatusCode.BadRequest, new
                            {
                                Username = username,
                                ClaimRequest = claimRequest
                            },
                            new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.OK, new
                                {
                                    Username = username,
                                    ClaimRequest = claimRequest

                                }, new List<DadosNotificacao> { new DadosNotificacao($"Claim {claimRequest.Type} / {claimRequest.Value}, adicionada com sucesso ao usuário {username}.") }));
                    });

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsavel por remover uma claim do usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="claimRequest"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<ObjectResult> RemoveUserClaimAsync(string username, ClaimRequest claimRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveUserClaimAsync)}\n");

            try
            {
                await new RemoveClaimValidator().ValidateAsync(
                    claimRequest).ContinueWith(async (validationTask) =>
                    {
                        var validation = validationTask.Result;

                        if (string.IsNullOrEmpty(username))
                            throw new CustomException(HttpStatusCode.BadRequest, null,
                               new List<DadosNotificacao> { new DadosNotificacao("Campo username deve ter um valor!") });

                        if (validation.IsValid is false)
                            await validation.GetValidationErrors();
                    });

                return await _userRepository.GetWithUsernameAsync(username).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                       userEntityTask.Result
                       ?? throw new NotFoundUserException(new
                       {
                           Username = username,
                           ClaimRequest = claimRequest
                       });

                    return await _userRepository.RemoveClaimUserAsync(userEntity, new Claim(claimRequest.Type, claimRequest.Value)).ContinueWith(identityResultTask =>
                    {
                        var identityResult = identityResultTask.Result;

                        if (identityResult.Succeeded is false)
                            throw new CustomException(HttpStatusCode.BadRequest, new
                            {
                                Username = username,
                                ClaimRequest = claimRequest
                            },
                            new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.OK, new
                                {
                                    Username = username,
                                    ClaimRequest = claimRequest

                                }, new List<DadosNotificacao> { new DadosNotificacao($"Claim {claimRequest.Type} / {claimRequest.Value}, removida com sucesso ao usuário {username}.") }));
                    });

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsavel por adicionar uma role ao usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<ObjectResult> AddUserRoleAsync(string username, string roleName)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddUserRoleAsync)}\n");

            try
            {
                if (string.IsNullOrEmpty(username))
                    throw new CustomException(HttpStatusCode.BadRequest, null,
                       new List<DadosNotificacao> { new DadosNotificacao("O campo nome de usuário deve ter um valor!") });

                if (string.IsNullOrEmpty(roleName))
                    throw new CustomException(HttpStatusCode.BadRequest, null,
                       new List<DadosNotificacao> { new DadosNotificacao("O campo nome da role deve ter um valor!") });

                return await _userRepository.GetWithUsernameAsync(username).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                        userEntityTask.Result
                        ?? throw new NotFoundUserException(new
                        {
                            Username = username,
                            RoleName = roleName

                        });

                    return await _userRepository.GetRoleAsync(roleName).ContinueWith(async (roleEntityTask) =>
                    {
                        var roleEntity =
                            roleEntityTask.Result
                            ?? throw new NotFoundUserException(new
                            {
                                Username = username,
                                RoleName = roleName

                            }, new List<DadosNotificacao>
                            {
                                new DadosNotificacao("Role não foi encontrada!")
                            });

                        return await _userRepository.AddToUserRoleAsync(userEntity, roleName).ContinueWith(identityResultTask =>
                        {
                            var identityResult = identityResultTask.Result;

                            if (identityResult.Succeeded is false)
                                throw new CustomException(HttpStatusCode.BadRequest, new
                                {
                                    Username = username,
                                    RoleName = roleName
                                },
                                new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });

                            return new OkObjectResult(
                                new ApiResponse<object>(
                                    identityResult.Succeeded, HttpStatusCode.OK, new
                                    {
                                        Username = username,
                                        RoleName = roleName

                                    }, new List<DadosNotificacao> { new DadosNotificacao($"Role {roleName}, adicionada com sucesso ao usuário {username}.") }));
                        });

                    }).Result;

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsável por recuperar roles de um usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetUserRolesAsync(Guid userId)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetUserRolesAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando todas as roles do usuário.\n");

                var userEntity = await _userRepository.GetByAsync(userId);

                if (userEntity is not null)
                {
                    var userRoles = await _userRepository.GetUserRolesAsync(userEntity);

                    var roles = new List<RolesResponse>();

                    foreach (var roleName in userRoles)
                    {
                        var roleEntity = await _userRepository.GetRoleAsync(roleName);

                        var roleClaims = await _userRepository.GetRoleClaimsAsync(roleEntity);

                        roles.Add(new RolesResponse { Name = roleName, Claims = roleClaims });
                    }

                    Log.Information($"[LOG INFORMATION] - Roles recuperadas.\n");

                    return new OkObjectResult(
                        new ApiResponse<object>(
                            true, HttpStatusCode.OK, roles.ToList(), new List<DadosNotificacao> { new DadosNotificacao("Roles recuperadas com sucesso.") }));
                }
                else
                {
                    Log.Information($"[LOG ERROR] - Usuário não encontrado.\n");

                    return new NotFoundObjectResult(
                        new ApiResponse<object>(
                            false, HttpStatusCode.NotFound, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário não encontrado.") }));
                }
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.InnerException} - {exception.Message}\n");

                return new ObjectResult(
                    new ApiResponse<object>(
                        false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) }));
            }
        }

        /// <summary>
        /// Método responsavel por remover uma role ao usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<ObjectResult> RemoveUserRoleAsync(string username, string roleName)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveUserRoleAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando dados do usuário {username}.\n");

                var userEntity = await _userRepository.GetWithUsernameAsync(username);

                if (userEntity is not null)
                {
                    Log.Information($"[LOG INFORMATION] - Removendo role ({roleName}) do usuário.\n");

                    var response = await _userRepository.RemoveToUserRoleAsync(userEntity, roleName);

                    if (response.Succeeded)
                    {
                        Log.Information($"[LOG INFORMATION] - Role removida com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                response.Succeeded, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Role {roleName}, removida com sucesso do usuário {username}.") }));
                    }

                    Log.Information($"[LOG INFORMATION] - Falha ao remover role.\n");

                    return new ObjectResult(
                        new ApiResponse<object>(
                            response.Succeeded, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao("Falha ao remover role.!") }));
                }
                else
                {
                    Log.Information($"[LOG ERROR] - Usuário não encontrado.\n");

                    return new NotFoundObjectResult(
                        new ApiResponse<object>(
                            false, HttpStatusCode.NotFound, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário não encontrado.") }));
                }
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                return new ObjectResult(
                    new ApiResponse<object>(
                        false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) }));
            }
        }

        /// <summary>
        /// Método responsavel por gerar um token de autorização e enviar por e-mail.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task ConfirmeUserForEmailAsync(UserEntity user)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(ConfirmeUserForEmailAsync)}\n");

            Log.Information($"[LOG INFORMATION] - Gerando codigo de confirmação de e-mail.\n");

            var confirmationCode = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

            var codifyEmailCode = HttpUtility.UrlEncode(confirmationCode).Replace("%", ";");

            Log.Information($"[LOG INFORMATION] - Código gerado - {codifyEmailCode}.\n");

            SendUserEmailToServiceBusJob.Execute(new UserEmailMessageDto
            {
                Receivers = new List<string> { user.Email },
                Link = $"{_appsettings.Value.UrlBase.TOOLS_WEB_APP}/confirmEmail/{codifyEmailCode}/{user.Id}",
                Subject = "Ativação de e-mail",
                Content = $"{user.UserName}, estamos muito felizes com o seu cadastro em nosso sistema. Clique no botão para liberarmos o seu acesso.",
                ButtonText = "Liberar acesso",
                TemplateName = "Activate.Template"
            });
        }

        /// <summary>
        /// Método responsável por tratar os erros de autenticação.
        /// </summary>
        /// <param name="signInResult"></param>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        /// <exception cref="LockedOutAuthenticationException"></exception>
        /// <exception cref="IsNotAllowedAuthenticationException"></exception>
        /// <exception cref="RequiresTwoFactorAuthenticationException"></exception>
        /// <exception cref="InvalidUserAuthenticationException"></exception>
        private static void ThrownAuthorizationException(SignInResult signInResult, LoginRequest loginRequest)
        {
            if (signInResult.IsLockedOut)
            {
                Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário, está bloqueado.\n");

                throw new LockedOutAuthenticationException(loginRequest);
            }
            else if (signInResult.IsNotAllowed)
            {
                Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário, não está confirmado.\n");

                throw new IsNotAllowedAuthenticationException(loginRequest);
            }
            else if (signInResult.RequiresTwoFactor)
            {
                Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário, requer verificação de dois fatores.\n");

                throw new RequiresTwoFactorAuthenticationException(loginRequest);
            }
            else
            {
                Log.Information($"[LOG INFORMATION] - Falha na autenticação dados incorretos!\n");

                throw new InvalidUserAuthenticationException(loginRequest);
            }
        }

        /// <summary>
        /// Método responsavel por gerar um tokenJwt.
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        private async Task<TokenJWT> GenerateTokenJwtAsync(UserEntity userEntity, LoginRequest loginRequest)
        {
            return await _tokenService.CreateJsonWebToken(loginRequest.Username).ContinueWith(async (tokenTask) =>
            {
                var tokenJwt = tokenTask.Result;

                await _userRepository
                    .SetUserAuthenticationTokenAsync(userEntity, "LOCAL-TOKEN", "AUTHENTICATION-TOKEN", tokenJwt.Value);

                return tokenJwt;

            }).Result;
        }

        /// <summary>
        /// Método responsavel por criar uma nova role.
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse<object>> CreateRoleAsync(RoleRequest roleRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(CreateAsync)}\n");

            try
            {
                // Mapper to entity.
                var role = roleRequest.ToIdentityRole();

                Log.Information($"[LOG INFORMATION] - Criando nova Role {roleRequest.Name}\n");

                // Create a role in database.
                var response = await _roleManager.CreateAsync(role);

                // Is success enter.
                if (response.Succeeded)
                {
                    Log.Information($"[LOG INFORMATION] - Adicionando claims na role {roleRequest.Name}\n");

                    foreach (var claim in roleRequest.Claims)
                    {
                        await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                    }

                    // Response success.
                    return new ApiResponse<object>(response.Succeeded, HttpStatusCode.Created, null, new List<DadosNotificacao> { new DadosNotificacao("Role criado com sucesso.") });
                }

                Log.Information($"[LOG INFORMATION] - Falha ao criar role.\n");

                // Response error.
                return new ApiResponse<object>(response.Succeeded, HttpStatusCode.BadRequest, null, response.Errors.Select(e => new DadosNotificacao(e.Description)).ToList());
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.InnerException} - {exception.Message}\n");

                // Error response.
                return new ApiResponse<object>(false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) });
            }
        }

        /// <summary>
        /// Método responsavel por retornar todas as roles.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<object>> GetRolesAsync()
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetByIdAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando todas as roles\n");

                // Get roles.
                var roles = await _roleManager.Roles.ToListAsync();

                // Response success.
                return new ApiResponse<object>(true, HttpStatusCode.OK, roles, new List<DadosNotificacao> { new DadosNotificacao("Roles recuperadas com sucesso.") });
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.InnerException} - {exception.Message}\n");

                // Error response.
                return new ApiResponse<object>(false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) });
            }
        }

        /// <summary>
        /// Método responsavel por adicionar uma claim na role.
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse<object>> AddClaimsToRoleAsync(RoleRequest roleRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddClaimsToRoleAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Adicionando uma novas claims na role {roleRequest.Name}\n");

                // Get the role for Id.
                var role = await _roleManager.Roles.FirstOrDefaultAsync(role => roleRequest.Name.Equals(role.Name));

                // Verify is nor null role.
                if (role is not null)
                {
                    // foreach claims.
                    foreach (var claim in roleRequest.Claims)
                    {
                        // Add claims in role.
                        await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));

                        Log.Information($"[LOG INFORMATION] - Claim {claim.Type}/{claim.Value} adicionada.\n");
                    }

                    // Response success.
                    return new ApiResponse<object>(true, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Claim adicionada a role {roleRequest.Name} com sucesso.") });
                }

                // Response error.
                return new ApiResponse<object>(false, HttpStatusCode.NotFound, null, new List<DadosNotificacao> { new DadosNotificacao($"Role com o nome {roleRequest.Name} não existe.") });
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.InnerException} - {exception.Message}\n");

                // Error response.
                return new ApiResponse<object>(false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) });
            }
        }

        /// <summary>
        /// Método responsável por remover uma claim na role.
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse<object>> RemoveClaimsToRoleAsync(RoleRequest roleRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveClaimsToRoleAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Removendo as claims da Role {roleRequest.Name}\n");

                // Get role for Id.
                var role = await _roleManager.Roles.FirstOrDefaultAsync(role => roleRequest.Name.Equals(role.Name));

                // Verify is not null role.
                if (role is not null)
                {
                    // Remove remove claim.
                    foreach (var claim in roleRequest.Claims)
                    {
                        await _roleManager.RemoveClaimAsync(role, new Claim(claim.Type, claim.Value));

                        Log.Information($"[LOG INFORMATION] - Claim {claim.Type} / {claim.Value} removida com sucesso.\n");
                    }

                    // Response success.
                    return new ApiResponse<object>(true, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Claim removida da role {roleRequest.Name} com sucesso.") });
                }

                Log.Information($"[LOG INFORMATION] - Role não existe.\n");

                return new ApiResponse<object>(false, HttpStatusCode.NotFound, null, new List<DadosNotificacao> { new DadosNotificacao($"Role com o nome {roleRequest.Name} não existe.") });
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                // Error response.
                return new ApiResponse<object>(false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) });
            }
        }
    }
}
