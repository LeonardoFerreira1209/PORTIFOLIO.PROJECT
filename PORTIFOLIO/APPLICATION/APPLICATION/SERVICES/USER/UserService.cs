using APPLICATION.APPLICATION.CONFIGURATIONS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.BASE;
using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.SENDGRID;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.EXCEPTIONS.BASE;
using APPLICATION.DOMAIN.FACTORY.MAIL;
using APPLICATION.DOMAIN.UTILS.Extensions;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.DOMAIN.VALIDATORS;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Net;
using System.Security.Claims;
using static APPLICATION.DOMAIN.EXCEPTIONS.CustomUserException;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace APPLICATION.APPLICATION.SERVICES.USER;

/// <summary>
/// Serviço de usuários.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly RoleManager<Role> _roleManager;
    private readonly IOptions<AppSettings> _appsettings;
    private readonly ITokenService _tokenService;
    private readonly IMailService<SendGridMailRequest, ApiResponse<object>> _mailService;
    private readonly IFileService _fileService;

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="appsettings"></param>
    /// <param name="tokenService"></param>
    public UserService(
        IUnitOfWork unitOfWork, IUserRepository userRepository, 
        IEventRepository eventRepository, RoleManager<Role> roleManager, 
        IOptions<AppSettings> appsettings, ITokenService tokenService, 
        IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _roleManager = roleManager;
        _appsettings = appsettings;
        _tokenService = tokenService;
        _fileService = fileService;

        SendGridMailFactory sendGridMailFactory = new(appsettings);
        _mailService =
            sendGridMailFactory.CreateMailService<SendGridMailRequest, ApiResponse<object>>();
    }

    /// <summary>
    /// Método responsável por fazer a authorização do usuário.
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundUserException"></exception>
    public async Task<ObjectResult> AuthenticationAsync(LoginRequest loginRequest)
    {
        Log.Information(
            $"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AuthenticationAsync)}\n");

        try
        {
            await new AuthenticationValidator().ValidateAsync(
                loginRequest).ContinueWith(async (validationTask) =>
                {
                    var validation = validationTask.Result;

                    if (validation.IsValid is false) await validation.GetValidationErrors();

                }).Unwrap();

            var tokenJWT = await _userRepository.GetWithUsernameAsync(
                loginRequest.Username).ContinueWith(async (userEntityTask) =>
                {
                    var userEntity =
                        userEntityTask.Result
                        ?? throw new NotFoundUserException(loginRequest);

                    await _userRepository.PasswordSignInAsync(
                        userEntity, loginRequest.Password, true, true).ContinueWith((signInResultTask) =>
                        {
                            var signInResult = signInResultTask.Result;

                            if (signInResult.Succeeded is false) ThrownAuthorizationException(signInResult, userEntity.Id, loginRequest);
                        });

                    Log.Information(
                        $"[LOG INFORMATION] - Usuário autenticado com sucesso!\n");

                    return await GenerateTokenJwtAsync(loginRequest).ContinueWith(
                        (tokenJwtTask) =>
                        {
                            var tokenJWT =
                                tokenJwtTask.Result
                                ?? throw new TokenJwtException(null);

                            Log.Information(
                                $"[LOG INFORMATION] - Token gerado com sucesso {JsonConvert.SerializeObject(tokenJWT)}!\n");

                            return tokenJWT;
                        });

                }).Unwrap();

            return new OkObjectResult(
                new ApiResponse<TokenJWT>(
                    true, HttpStatusCode.Created, tokenJWT, new List<DadosNotificacao>  {
                        new DadosNotificacao("Token criado com sucesso!")
                    }));
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por gerar um novo tokenJwt.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<ObjectResult> RefreshTokenAsync(string refreshToken)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RefreshTokenAsync)}\n");

        try
        {
            return await _tokenService.CreateJsonWebTokenByRefreshToken(refreshToken).ContinueWith((tokenJWTResult) =>
            {
                var tokenJWT
                    = tokenJWTResult.Result
                    ?? throw new TokenJwtException(refreshToken);

                return new OkObjectResult(
                    new ApiResponse<TokenJWT>(
                        true, HttpStatusCode.Created, tokenJWT, new List<DadosNotificacao>  {
                            new DadosNotificacao("Token criado com sucesso!")
                        })
                    );
            });
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
            return await _userRepository.GetByIdAsync(userId).ContinueWith(userEntityTask =>
            {
                var userEntity =
                    userEntityTask.Result
                    ?? throw new NotFoundUserException(userId);

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
    /// Método responsável por recuperar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundUserException"></exception>
    public async Task<ObjectResult> UpdateImageAsync(Guid userId, IFormFile formFile)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(UpdateImageAsync)}\n");

        try
        {
            return await _userRepository.GetByIdAsync(userId).ContinueWith(
                async (userEntityTask) =>
                {
                    var userEntity =
                        userEntityTask.Result
                        ?? throw new NotFoundUserException(userId);

                    return await _fileService.UploadAsync(userId, formFile).ContinueWith(
                        async (taskFileResult) =>
                        {
                            var file = taskFileResult.Result;

                            userEntity.File = file;

                            await _userRepository.UpdateUserAsync(userEntity);

                            return new OkObjectResult(
                                new ApiResponse<UserResponse>(
                                    true, HttpStatusCode.OK, userEntity.ToResponse(), new List<DadosNotificacao> { new DadosNotificacao("Imagenm do usu com sucesso.") })
                                );

                        }).Unwrap();

                }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    ///  Método responsável por buscar usuários através de um nome.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetUsersByNameAsync(string name)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetUsersByNameAsync)}\n");

        try
        {
            if (name is not null)
            {
                var names =
                    new List<string> { name };

                return await _userRepository.GetByNamesAsync(names).ContinueWith(userEntityTask =>
                {
                    var usersEntity =
                        userEntityTask.Result.ToList();

                    var usersResponse =
                            usersEntity.Select(
                                user => user.ToResponse()).ToList();

                    return new OkObjectResult(
                        new ApiResponse<List<UserResponse>>(
                            true, HttpStatusCode.OK, usersResponse, new List<DadosNotificacao> { new DadosNotificacao("Usuarios recuperados com sucesso.") })
                        );
                });
            }
            else
            {
                return new OkObjectResult(
                    new ApiResponse<List<object>>(
                        true, HttpStatusCode.BadRequest, null, new List<DadosNotificacao> { new DadosNotificacao("O nome do usuário é nulo, envie um nome válido!") })
                    );
            }
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

                    await _userRepository.IsCpfAlreadyRegistered(userCreateRequest.CPF).ContinueWith((existTask) =>
                    {
                        if (existTask.Result)
                        {
                            validation.Errors.Add(new ValidationFailure
                            {
                                ErrorCode = "400",
                                Severity = Severity.Warning,
                                ErrorMessage = "CPF já registrado!"
                            });
                        }
                    });

                    if (validation.IsValid is false) await validation.GetValidationErrors();

                }).Unwrap();

            var user =
                userCreateRequest.ToIdentityUser();

            return await _userRepository.CreateUserAsync(
                user, userCreateRequest.Password).ContinueWith(async (identityResultTask) =>
                {
                    var identityResult = identityResultTask.Result;

                    if (identityResult.Succeeded is false)
                        throw new CustomException(
                            HttpStatusCode.BadRequest, userCreateRequest, identityResult.Errors.Select((e) => new DadosNotificacao(e.Code.CustomExceptionMessage())).ToList());

                    var userEntity =
                        await _userRepository.GetWithUsernameAsync(userCreateRequest.UserName);

                    await SendConfirmationEmailCode(user);

                    return new OkObjectResult(
                        new ApiResponse<object>(
                            identityResult.Succeeded, HttpStatusCode.Created, new
                            {
                                userEntity.Id,
                                userEntity.UserName

                            }, new List<DadosNotificacao> { new DadosNotificacao("Usuário criado com sucesso.") }));

                }).Result;
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
                      await validation.GetValidationErrors(userUpdateRequest);

              }).Unwrap();

            return await _userRepository.GetByIdAsync(userUpdateRequest.Id).ContinueWith(async (userEntityTask) =>
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

                var emailConfirmed = true;
                if (userUpdateRequest.Email.Equals(userEntity.Email) is false)
                {
                    await _userRepository.SetEmailAsync(
                        userEntity, userUpdateRequest.Email).ContinueWith(async (identityResultTask) =>
                        {
                            var identityResult = identityResultTask.Result;

                            emailConfirmed = false;
                            await SendConfirmationEmailCode(userEntity);

                            if (identityResult.Succeeded is false)
                                throw new CustomException(HttpStatusCode.BadRequest, userUpdateRequest,
                                    new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                        }).Result;
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
                            userUpdateRequest, emailConfirmed));

                return new OkObjectResult(
                   new ApiResponse<object>(
                       true, HttpStatusCode.OK, userEntity.ToResponse(),
                       new List<DadosNotificacao> { new DadosNotificacao("Usuário atualizado com sucesso.") }));

            }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsavel por ativar um novo usuário.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundUserException"></exception>
    /// <exception cref="CustomException"></exception>
    public async Task<ObjectResult> ActivateUserAsync(Guid userId, string code)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(ActivateUserAsync)}\n");

        try
        {
            return await _userRepository.GetUserConfirmationCode(userId, code).ContinueWith(
                async (codeTask) =>
                {
                    var userCode =
                        codeTask.Result ?? throw new IncorrectConfirmationCodeAuthenticationException(new { userId, code });

                    if (userCode.Status is Status.Inactive) throw new IncorrectConfirmationCodeAuthenticationException(new { userId, code });

                    await _userRepository.GetByIdAsync(userCode.UserId).ContinueWith(
                        async (userEntityTask) =>
                        {
                            var userEntity =
                                userEntityTask.Result
                                ?? throw new NotFoundUserException(userId);

                            await _userRepository.ConfirmEmailAsync(userEntity, userCode.HashCode).ContinueWith(
                                identityResultTask =>
                                {
                                    var identityResult
                                        = identityResultTask.Result;

                                    if (identityResult.Succeeded)
                                    {
                                        userCode.Updated = DateTime.Now;
                                        userCode.Status = Status.Inactive;

                                        _userRepository.UpdateUserConfirmationCode(userCode);
                                    }
                                    else
                                        throw new CustomException(HttpStatusCode.BadRequest, code,
                                            new List<DadosNotificacao> { new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage()) });
                                });

                        }).Unwrap();

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
                      await validation.GetValidationErrors(claimRequest);

              }).Unwrap();

            return await _userRepository.GetWithUsernameAsync(username).ContinueWith(async (userEntityTask) =>
            {
                var userEntity =
                   userEntityTask.Result
                   ?? throw new NotFoundUserException(new
                   {
                       Username = username,
                       ClaimRequest = claimRequest
                   });

                return await _userRepository.AddClaimUserAsync(
                    userEntity, new Claim(claimRequest.Type, claimRequest.Value)).ContinueWith(identityResultTask =>
                    {
                        var identityResult
                            = identityResultTask.Result;

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

            }).Unwrap();
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
                        await validation.GetValidationErrors(claimRequest);

                }).Unwrap();

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
                    var identityResult
                        = identityResultTask.Result;

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

            }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsavel por adicionar uma role ao usuário.
    /// </summary>
    /// <param name="userRoleRequest"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundUserException"></exception>
    /// <exception cref="CustomException"></exception>
    public async Task<ObjectResult> AddUserRoleAsync(UserRoleRequest userRoleRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddUserRoleAsync)}\n");

        try
        {
            return await _userRepository.GetWithUsernameAsync(userRoleRequest.Username).ContinueWith(async (userEntityTask) =>
            {
                var userEntity =
                    userEntityTask.Result
                    ?? throw new NotFoundUserException(userRoleRequest);

                return await _userRepository.GetRoleAsync(userRoleRequest.RoleName).ContinueWith(async (roleEntityTask) =>
                {
                    var roleEntity =
                        roleEntityTask.Result
                        ?? throw new NotFoundUserException(userRoleRequest, new List<DadosNotificacao> {
                            new DadosNotificacao("Role não foi encontrada!")
                        });

                    return await _userRepository.AddToUserRoleAsync(userEntity, userRoleRequest.RoleName).ContinueWith(identityResultTask =>
                    {
                        var identityResult
                            = identityResultTask.Result;

                        if (identityResult.Succeeded is false)
                            throw new CustomException(HttpStatusCode.BadRequest, userRoleRequest, new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage())
                            });

                        return new OkObjectResult(
                            new ApiResponse<object>(
                                identityResult.Succeeded, HttpStatusCode.OK, userRoleRequest,
                                new List<DadosNotificacao> { new DadosNotificacao($"Role {userRoleRequest.RoleName}, adicionada com sucesso ao usuário {userRoleRequest.Username}.") }));
                    });

                }).Unwrap();

            }).Unwrap();
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
            return await _userRepository.GetByIdAsync(userId).ContinueWith(async (userEntityTask) =>
            {
                var userEntity =
                    userEntityTask.Result
                    ?? throw new NotFoundUserException(userId);

                return await _userRepository.GetUserRolesAsync(userEntity).ContinueWith(userRolesTask =>
                {
                    List<string> userRoles = userRolesTask.Result.ToList();

                    var roles = new List<RolesResponse>();

                    userRoles.ForEach(async (roleName) =>
                    {
                        await _userRepository.GetRoleAsync(roleName).ContinueWith(async (roleEntityTask) =>
                        {
                            var roleEntity = roleEntityTask.Result;

                            await _userRepository.GetRoleClaimsAsync(roleEntity).ContinueWith(claimsResult =>
                            {
                                var claims = claimsResult.Result;

                                roles.Add(new RolesResponse { Name = roleName, Claims = claims });
                            });
                        });
                    });

                    return new OkObjectResult(
                        new ApiResponse<List<RolesResponse>>(
                            true, HttpStatusCode.OK, roles,
                                new List<DadosNotificacao> { new DadosNotificacao("Roles recuperadas com sucesso!") }));
                });

            }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
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
            return await _userRepository.GetWithUsernameAsync(username).ContinueWith(async (userEntityTask) =>
            {
                var userEntity =
                   userEntityTask.Result
                   ?? throw new NotFoundUserException(username);

                return await _userRepository.RemoveToUserRoleAsync(userEntity, roleName).ContinueWith(identityResultTask =>
                {
                    var identityResult
                        = identityResultTask.Result;

                    if (identityResult.Succeeded is false)
                        throw new CustomException(HttpStatusCode.BadRequest, new { username, roleName }, new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage())
                        });

                    return new OkObjectResult(
                          new ApiResponse<object>(
                            identityResult.Succeeded, HttpStatusCode.OK, new { username, roleName },
                            new List<DadosNotificacao> { new DadosNotificacao($"Role {roleName}, removida com sucesso do usuário {username}.") }));
                });

            }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por tratar os erros de autenticação.
    /// </summary>
    /// <param name="signInResult"></param>
    /// <returns></returns>
    /// <exception cref="LockedOutAuthenticationException"></exception>
    /// <exception cref="IsNotAllowedAuthenticationException"></exception>
    /// <exception cref="RequiresTwoFactorAuthenticationException"></exception>
    /// <exception cref="InvalidUserAuthenticationException"></exception>
    private static void ThrownAuthorizationException(SignInResult signInResult, Guid userId, LoginRequest loginRequest)
    {
        if (signInResult.IsLockedOut)
        {
            Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário, está bloqueado.\n");

            throw new LockedOutAuthenticationException(loginRequest);
        }
        else if (signInResult.IsNotAllowed)
        {
            Log.Information($"[LOG INFORMATION] - Falha ao recuperar usuário, não está confirmado.\n");

            throw new IsNotAllowedAuthenticationException(new
            {
                userId,
                isNotAllowed = true,
                loginRequest
            });
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
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    private async Task<TokenJWT> GenerateTokenJwtAsync(LoginRequest loginRequest)
    {
        return await _tokenService.CreateJsonWebToken(loginRequest.Username).ContinueWith((tokenTask) =>
        {
            var tokenJwt =
                tokenTask.Result
                ?? throw new TokenJwtException(loginRequest);

            return tokenJwt;
        });
    }

    /// <summary>
    /// Método responsavel por criar uma nova role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    public async Task<ObjectResult> CreateRoleAsync(RoleRequest roleRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(CreateRoleAsync)}\n");

        try
        {
            var role = roleRequest.ToIdentityRole();

            return await _roleManager.CreateAsync(role).ContinueWith(
                async (idenityResultTask) =>
                {
                    var identityResult = idenityResultTask.Result;

                    if (identityResult.Succeeded is false)
                        throw new CustomException(HttpStatusCode.BadRequest, roleRequest, new List<DadosNotificacao> {
                                new DadosNotificacao(identityResult.Errors.FirstOrDefault()?.Code.CustomExceptionMessage())
                        });

                    foreach (var claim in roleRequest.Claims)
                        await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));

                    return new OkObjectResult(
                        new ApiResponse<object>(
                            identityResult.Succeeded, HttpStatusCode.OK, roleRequest,
                            new List<DadosNotificacao> { new DadosNotificacao($"Role {roleRequest.Name}, Role criado com sucesso.") }));

                }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsavel por retornar todas as roles.
    /// </summary>
    /// <returns></returns>
    public async Task<ObjectResult> GetRolesAsync()
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(GetByIdAsync)}\n");

        try
        {
            return await _roleManager.Roles.ToListAsync().ContinueWith(
                (rolesEntityTask) =>
                {
                    var roles = rolesEntityTask.Result;

                    return new OkObjectResult(
                         new ApiResponse<object>(
                             true, HttpStatusCode.OK, roles));
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsavel por adicionar uma claim na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    public async Task<ObjectResult> AddClaimsToRoleAsync(RoleRequest roleRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(AddClaimsToRoleAsync)}\n");

        try
        {
            return await _roleManager.Roles.FirstOrDefaultAsync(
                 role => roleRequest.Name.Equals(role.Name)).ContinueWith(async (roleTask) =>
                 {
                     var role =
                         roleTask.Result
                         ?? throw new NotFoundRoleException(roleRequest);

                     foreach (var claim in roleRequest.Claims)
                         await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));

                     return new OkObjectResult(
                        new ApiResponse<object>(
                            true, HttpStatusCode.OK, role,
                            new List<DadosNotificacao> { new DadosNotificacao($"Claims adicionada a Role {roleRequest.Name} com sucesso!") }));

                 }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por remover uma claim na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    public async Task<ObjectResult> RemoveClaimsToRoleAsync(RoleRequest roleRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(RemoveClaimsToRoleAsync)}\n");

        try
        {
            return await _roleManager.Roles.FirstOrDefaultAsync(
                role => roleRequest.Name.Equals(role.Name)).ContinueWith(async (roleTask) =>
                {
                    var role =
                            roleTask.Result
                            ?? throw new NotFoundRoleException(roleRequest);

                    foreach (var claim in roleRequest.Claims)
                        await _roleManager.RemoveClaimAsync(role, new Claim(claim.Type, claim.Value));

                    return new OkObjectResult(
                       new ApiResponse<object>(
                           true, HttpStatusCode.OK, role,
                           new List<DadosNotificacao> { new DadosNotificacao($"Claims removidas da Role {roleRequest.Name} com sucesso!") }));

                }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message}  -  {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Envie a mensagem de confirmação.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task SendConfirmationEmailCode(User user)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(UserService)} - METHOD {nameof(SendConfirmationEmailCode)}\n");

        var confirmationCodeIdentity
            = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

        var userCodeEntity = await _userRepository.AddUserConfirmationCode(
            new UserCode
            {
                Created = DateTime.Now,
                NumberCode = confirmationCodeIdentity.HashCode(),
                HashCode = confirmationCodeIdentity,
                Status = Status.Active,
                UserId = user.Id
            });

        await _mailService.SendSingleMailWithTemplateAsync(
            new EmailAddress(),
            new EmailAddress(
        user.FirstName, user.Email), "d-a5a2d227be3a491ea863112e28b2ae84", new
        {
            name = user.FirstName,
            code = userCodeEntity.NumberCode

        }).ContinueWith(async (mailResultTask) =>
        {
            if (mailResultTask.Result.Sucesso is false) {
                var eventFail = await _eventRepository.CreateAsync(EventExtensions.CreateMailEvent(
                   "FailedToSendConfirmationMail", "Reenvio de e-mail de confirmação de usuário.", new
                   {
                       From = new EmailAddress(),
                       To = new EmailAddress(user.FirstName, user.Email),
                       TemplateId = "d-a5a2d227be3a491ea863112e28b2ae84",
                       DynamicTemplateData = new
                       {
                           Name = user.FirstName,
                           Code = userCodeEntity.NumberCode
                       }
                   }));

                Log.Information(
                    $"[LOG INFORMATION] - Envio de e-mail falhou, evento {JsonConvert.SerializeObject(eventFail)}, de reenvio de e-mail criado com sucesso!\n");
            }
            else
            {
                Log.Information(
                    $"[LOG INFORMATION] - E-mail de confirmação enviado com sucesso!\n");
            }

            await _unitOfWork.CommitAsync();

        }).Unwrap();
    }
}
