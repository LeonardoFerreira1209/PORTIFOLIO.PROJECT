using APPLICATION.APPLICATION.CONFIGURATIONS;
using APPLICATION.DOMAIN.CONTRACTS.FACADE;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.TOKEN;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.SERVICEBUS.MESSAGE;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.FILE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER.ROLE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.EXCEPTIONS.USER;
using APPLICATION.DOMAIN.UTILS.Extensions;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.DOMAIN.VALIDATORS;
using APPLICATION.INFRAESTRUTURE.JOBS.QUEUED;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public UserService(IUserRepository userRepository, IOptions<AppSettings> appsettings, ITokenService tokenService, IUtilFacade utilFacade)
        {
            _userRepository = userRepository;
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
                await new AuthenticationValidator().ValidateAsync(loginRequest).ContinueWith(async (validationTask) =>
                {
                    var validation = validationTask.Result;

                    if (validation.IsValid is false)
                        await validation.CarregarErrosValidator();
                });

                return await _userRepository.GetWithUsernameAsync(loginRequest.Username).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity = userEntityTask.Result;

                    if (userEntity is not null)
                    {
                        await _userRepository.PasswordSignInAsync(userEntity, loginRequest.Password, true, true).ContinueWith((signInResultTask) =>
                        {
                            var signInResult = signInResultTask.Result;

                            if (signInResult.Succeeded is false)
                                ThrownAuthorizationException(signInResult, loginRequest);
                        });
                    }
                    else
                    {
                        Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário!\n");

                        throw new NotFoundUserException(loginRequest);
                    }

                    var tokenJWT = await GenerateTokenJwtAsync(userEntity, loginRequest);

                    return new OkObjectResult(
                        new ApiResponse<TokenJWT>(
                            true, HttpStatusCode.Created, tokenJWT, new List<DadosNotificacao>
                            {
                                new DadosNotificacao("Token criado com sucesso!")
                            })
                        );

                }).Result;
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n"); throw;
            }
        }

        /// <summary>
        /// Método responsável por recuperar um usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetAsync(Guid userId)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando usuário com Id {userId}.\n");

                var userEntity = await _userRepository.GetAsync(userId);

                if (userEntity is not null)
                {
                    var userResponse = userEntity.ToResponse();

                    Log.Information($"[LOG INFORMATION] - Usuário recuperado com sucesso {JsonConvert.SerializeObject(userResponse)}.\n");

                    return new OkObjectResult(
                        new ApiResponse<UserResponse>(
                            true, HttpStatusCode.OK, userResponse, new List<DadosNotificacao> { new DadosNotificacao("Usuario recuperado com sucesso.") }));
                }

                Log.Information($"[LOG INFORMATION] - Usuário não encontrado.\n");

                throw new NotFoundUserException(userId);
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                throw;
            }
        }

        /// <summary>
        /// Método responsavel por criar um novo usuário.
        /// </summary>
        /// <param name="userCreateRequest"></param>
        /// <returns></returns>
        public async Task<ObjectResult> CreateAsync(UserCreateRequest userCreateRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(CreateAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Validando request.\n");

                var validation = await new CreateUserValidator().ValidateAsync(userCreateRequest); if (validation.IsValid is false) await validation.CarregarErrosValidator(userCreateRequest);

                Log.Information($"[LOG INFORMATION] - Request validado com sucesso.\n");

                var user = userCreateRequest.ToIdentityUser();

                var response = await BuildUserAsync(user, userCreateRequest.Password);

                if (response.Succeeded)
                {
                    await ConfirmeUserForEmailAsync(user);

                    return new OkObjectResult(
                        new ApiResponse<object>(
                            response.Succeeded, HttpStatusCode.Created, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário criado com sucesso.") }));
                }

                throw new CustomException(
                    HttpStatusCode.BadRequest, userCreateRequest, response.Errors.Select((e) => new DadosNotificacao(e.Code.CustomExceptionMessage())).ToList());
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                throw;
            }
        }

        /// <summary>
        /// Método responsável por atualizar um usuário.
        /// </summary>
        /// <param name="userUpdateRequest"></param>
        /// <returns></returns>
        public async Task<ObjectResult> UpdateAsync(UserUpdateRequest userUpdateRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(UpdateAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Validando request.\n");

                var validation = await new UpdateUserValidator().ValidateAsync(userUpdateRequest); if (validation.IsValid is false) await validation.CarregarErrosValidator(userUpdateRequest);

                Log.Information($"[LOG INFORMATION] - Request validado com sucesso.\n");

                Log.Information($"[LOG INFORMATION] - Atualizando dados do usuário {JsonConvert.SerializeObject(userUpdateRequest)}.\n");

                var userEntity = await _userRepository.GetAsync(userUpdateRequest.Id);

                if (userEntity is not null)
                {
                    if (!userUpdateRequest.UserName.Equals(userEntity.UserName))
                    {
                        var setUsernameResponse = await _userRepository.SetUserNameAsync(userEntity, userUpdateRequest.UserName);

                        if (setUsernameResponse.Succeeded is false)
                        {
                            Log.Information($"[LOG INFORMATION] - Erro ao atualizar nome de usuário.\n");

                            throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                new List<DadosNotificacao> { new DadosNotificacao(setUsernameResponse.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        }
                    }

                    if (!string.IsNullOrEmpty(userUpdateRequest.Password))
                    {
                        var changePasswordResponse = await _userRepository.ChangePasswordAsync(userEntity, userUpdateRequest.CurrentPassword, userUpdateRequest.Password);

                        if (changePasswordResponse.Succeeded is false)
                        {
                            Log.Information($"[LOG INFORMATION] - Erro ao trocar senha.\n");

                            throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                new List<DadosNotificacao> { new DadosNotificacao(changePasswordResponse.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        }
                    }

                    if (!userUpdateRequest.Email.Equals(userEntity.Email))
                    {
                        var setEmailResponse = await _userRepository.SetEmailAsync(userEntity, userUpdateRequest.Email);

                        if (setEmailResponse.Succeeded is false)
                        {
                            Log.Information($"[LOG INFORMATION] - Erro ao atualizar e-mail de usuário.\n");

                            throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                new List<DadosNotificacao> { new DadosNotificacao(setEmailResponse.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        }

                        await ConfirmeUserForEmailAsync(userEntity);
                    }

                    if (!userUpdateRequest.PhoneNumber.Equals(userEntity.PhoneNumber))
                    {
                        var setPhoneNumberResponse = await _userRepository.SetPhoneNumberAsync(userEntity, userUpdateRequest.PhoneNumber);

                        if (setPhoneNumberResponse.Succeeded is false)
                        {
                            Log.Information($"[LOG INFORMATION] - Erro ao atualizar celular do usuário.\n");

                            throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                new List<DadosNotificacao> { new DadosNotificacao(setPhoneNumberResponse.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        }
                    }

                    userEntity = userUpdateRequest.ToCompleteUserUpdateWithRequest(userEntity);

                    await _userRepository.UpdateUserAsync(userEntity);

                    Log.Information($"[LOG INFORMATION] - Usuário atualizado com sucesso.\n");

                    return new OkObjectResult(
                        new ApiResponse<object>(
                            true, HttpStatusCode.OK, userEntity, new List<DadosNotificacao> { new DadosNotificacao("Usuário atualizado com sucesso.") }));
                }
                else
                {
                    Log.Information($"[LOG INFORMATION] - Usuário não encontrado.\n");

                    throw new NotFoundUserException(userUpdateRequest);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                throw;
            }
        }

        /// <summary>
        /// Método responsável por atualizar a imagem de um usuário.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<ObjectResult> UpdateUserIamgeAsync(Guid id, IFormFile formFile)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(UpdateUserIamgeAsync)}\n");

            try
            {
                var userEntity = await _userRepository.GetAsync(id);

                if (userEntity is not null)
                {
                    var response = await _utilFacade.SendAsync(formFile);

                    if (response.Sucesso)
                    {
                        var fileResponse = (FileResponse)response.Dados;

                        //userEntity.ImageUri = fileResponse.FileUri;

                        await _userRepository.UpdateUserAsync(userEntity);

                        Log.Information($"[LOG INFORMATION] - Imagem do usuário atualizado com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                true, HttpStatusCode.OK, new FileResponse { FileUri = null }, new List<DadosNotificacao> { new DadosNotificacao("Imagem do usuário atualizado com sucesso.") }));
                    }
                    else
                    {
                        Log.Information($"[LOG INFORMATION] - Erro ao armazenar imagem no blob do azure.\n");

                        return new ObjectResult(
                            new ApiResponse<object>(
                                false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao("Erro ao armazenar imagem no blob do azure.") }));
                    }
                }
                else
                {
                    Log.Information($"[LOG INFORMATION] - Usuário não encontrado.\n");

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
        /// Método responsavel por ativar um novo usuário.
        /// </summary>
        /// <param name="activateUserRequest"></param>
        /// <returns></returns>
        public async Task<ObjectResult> ActivateAsync(ActivateUserRequest activateUserRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(ActivateAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando usuário.\n");

                var userEntity = await _userRepository.GetAsync(activateUserRequest.UserId);

                if (userEntity is not null)
                {
                    Log.Information($"[LOG INFORMATION] - Confirmando e-mail do usuário {userEntity.UserName}.\n");

                    var response = await _userRepository.ConfirmEmailAsync(userEntity, HttpUtility.UrlDecode(activateUserRequest.Code.Replace(";", "%")));

                    if (response.Succeeded)
                    {
                        Log.Information($"[LOG INFORMATION] - Usuário ativado com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                response.Succeeded, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário ativado com sucesso.") }));
                    }

                    Log.Information($"[LOG INFORMATION] - Falha na ativãção do usuário.\n");

                    return new ObjectResult(
                        new ApiResponse<object>(
                            false, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao("Falha na ativãção do usuário!") }));
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
        /// Método responsavel por criar uma nova claim para o usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="claimRequest"></param>
        /// <returns></returns>
        public async Task<ObjectResult> AddClaimAsync(string username, ClaimRequest claimRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddClaimAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando usuario {username}.\n");

                var userEntity = await _userRepository.GetWithUsernameAsync(username);

                if (userEntity is not null)
                {
                    Log.Information($"[LOG INFORMATION] - Adicionando a claim ({claimRequest.Type}/{claimRequest.Value}) no usuário.\n");

                    var identityResult = await _userRepository.AddClaimUserAsync(userEntity, new Claim(claimRequest.Type, claimRequest.Value));

                    if (identityResult.Succeeded)
                    {
                        Log.Information($"[LOG INFORMATION] - Claim adicionada com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Claim {claimRequest.Type} / {claimRequest.Value}, adicionada com sucesso ao usuário {username}.") }));
                    }

                    Log.Information($"[LOG ERROR] - Falha ao adicionar claim.\n");

                    return new ObjectResult(
                        new ApiResponse<object>(
                            identityResult.Succeeded, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao("Falha ao adicionar claim!") }));
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
        /// Método responsavel por remover uma claim do usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="claimRequest"></param>
        /// <returns></returns>
        public async Task<ObjectResult> RemoveClaimAsync(string username, ClaimRequest claimRequest)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveClaimAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando usuário {username}.\n");

                var userIdentity = await _userRepository.GetWithUsernameAsync(username);

                if (userIdentity is not null)
                {
                    Log.Information($"[LOG INFORMATION] - Removendo a claim ({claimRequest.Type}/{claimRequest.Value}) do usuário.\n");

                    var identityResult = await _userRepository.RemoveClaimUserAsync(userIdentity, new Claim(claimRequest.Type, claimRequest.Value));

                    if (identityResult.Succeeded)
                    {
                        Log.Information($"[LOG INFORMATION] - Claim remvida com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Claim {claimRequest.Type} / {claimRequest.Value}, removida com sucesso do usuário {username}.") }));
                    }

                    Log.Information($"[LOG ERROR] - Falha ao remover claim.\n");

                    return new ObjectResult(
                        new ApiResponse<object>(
                            identityResult.Succeeded, HttpStatusCode.InternalServerError, null, new List<DadosNotificacao> { new DadosNotificacao("Falha ao remover claim!") }));
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
        /// Método responsavel por adicionar uma role ao usuário.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<ObjectResult> AddRoleAsync(string username, string roleName)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddRoleAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando dados do usuário {username}.\n");

                var userEntity = await _userRepository.GetWithUsernameAsync(username);

                if (userEntity is not null)
                {
                    Log.Information($"[LOG INFORMATION] - Adicionando a role ({roleName}) ao usuário.\n");

                    var response = await _userRepository.AddToUserRoleAsync(userEntity, roleName);

                    if (response is not null && response.Succeeded)
                    {
                        Log.Information($"[LOG INFORMATION] - Role adicionada com sucesso.\n");

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                response.Succeeded, HttpStatusCode.OK, null, new List<DadosNotificacao> { new DadosNotificacao($"Role {roleName}, adicionada com sucesso ao usuário {username}.") }));
                    }
                    else
                    {
                        Log.Information($"[LOG ERROR] - Falha ao adicionar role.\n");

                        return new BadRequestObjectResult(
                            new ApiResponse<object>(
                                false, HttpStatusCode.BadRequest, null, new List<DadosNotificacao> { new DadosNotificacao("Falha ao adicionar role!") }));
                    }
                }

                Log.Information($"[LOG ERROR] - Usuário não encontrado.\n");

                return new NotFoundObjectResult(
                    new ApiResponse<object>(
                        false, HttpStatusCode.NotFound, null, new List<DadosNotificacao> { new DadosNotificacao("Usuário não encontrado!") }));
            }
            catch (Exception exception)
            {
                Log.Error($"[LOG ERROR] - {exception.Message}\n");

                return new ObjectResult(
                    new ApiResponse<object>(
                        false, HttpStatusCode.InternalServerError, new List<DadosNotificacao> { new DadosNotificacao(exception.Message) }));
            }
        }

        /// <summary>
        /// Método responsável por recuperar roles de um usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ObjectResult> GetUserRolesAsync(Guid userId)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(RoleService)} - METHOD {nameof(GetUserRolesAsync)}\n");

            try
            {
                Log.Information($"[LOG INFORMATION] - Recuperando todas as roles do usuário.\n");

                var userEntity = await _userRepository.GetAsync(userId);

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
        public async Task<ObjectResult> RemoveRoleAsync(string username, string roleName)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveRoleAsync)}\n");

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
        /// Método responsavel por gerar um usuário e vincular roles e claims.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<IdentityResult> BuildUserAsync(UserEntity user, string password)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(BuildUserAsync)}\n");

            Log.Information("[LOG INFORMATION] - Criando usuário\n");

            var identityResult = await _userRepository.CreateUserAsync(user, password);

            await _userRepository.UpdateUserAsync(user);

            return identityResult;
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

                await SetAuthenticationTokenAsync(userEntity, tokenJwt.Value);

                return tokenJwt;

            }).Result;
        }

        /// <summary>
        /// Método responsável por atualizar o token do usuário.
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task SetAuthenticationTokenAsync(UserEntity userEntity, string token)
        {
            await _userRepository.SetUserAuthenticationTokenAsync(userEntity, "EVENTHUB", "AUTHENTICATIONTOKEN", token);
        }
    }
}
