using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.ATTRIBUTE;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER.ROLE;
using APPLICATION.DOMAIN.ENUMS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PORTIFOLIO.API.CONTROLLER.BASE;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace PORTIFOLIO.API.CONTROLLER;

/// <summary>
/// UserManagerController
/// </summary>
[ApiController]
[Route("api/usermanager")]
[EnableCors("CorsPolicy")]
public class UserManagerController : BaseControllercs
{
    private readonly IUserService _userService;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="userService"></param>
    public UserManagerController(
        IFeatureFlags featureFlags, IUserService userService, IUnitOfWork unitOfWork) : base(featureFlags, unitOfWork)
    {
        _userService = userService;
    }

    /// <summary>
    /// Endpoint responsável por fazer a autenticação do usuário, é retornado um token JWT (Json Web Token).
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpGet("authetication")]
    [SwaggerOperation(Summary = "Autenticação do usuário", Description = "Endpoint responsável por fazer a autenticação do usuário, é retornado um token JWT (Json Web Token).")]
    [ProducesResponseType(typeof(ApiResponse<TokenJWT>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<LoginRequest>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginRequest>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<LoginRequest>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AuthenticationAsync([FromHeader][Required] string username, [FromHeader][Required] string password)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { username, password })))
        using (LogContext.PushProperty("Metodo", "Authentication"))
        {
            return await ExecuteAsync(nameof(AuthenticationAsync),
                () => _userService.AuthenticationAsync(new LoginRequest(username, password)), "Autenticar usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por gerar um novo token de autenticação JWT (Json Web Token), baseado em um refresh Token.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpGet("refreshtoken")]
    [SwaggerOperation(Summary = "Gerar token do usuário através de um refresh token", Description = "Endpoint responsável por gerar um novo token de autenticação JWT (Json Web Token), baseado em um refresh Token.")]
    [ProducesResponseType(typeof(ApiResponse<TokenJWT>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshTokenAsync([FromHeader][Required] string refreshToken)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(refreshToken)))
        using (LogContext.PushProperty("Metodo", "RefreshToken"))
        {
            return await ExecuteAsync(nameof(RefreshTokenAsync),
                 () => _userService.RefreshTokenAsync(refreshToken), "Gerar novo token através do Refresh Token");
        }
    }

    /// <summary>
    /// Endpoint responsável por criar um novo usuário.
    /// </summary>
    /// <param name="userCreateRequest"></param>
    /// <returns></returns>
    [HttpPost("create/user")]
    [SwaggerOperation(Summary = "Criar uauário.", Description = "Endpoint responsável por criar um novo usuário.")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserCreateRequest>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateRequest userCreateRequest)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userCreateRequest)))
        using (LogContext.PushProperty("Metodo", "Create"))
        {
            return await ExecuteAsync(nameof(CreateAsync),
                 () => _userService.CreateAsync(userCreateRequest), "Criar novo usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por localizar um registro de usuário através do nome.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("get/users/by/name")]
    [CustomAuthorize(Claims.User, "Get")]
    [SwaggerOperation(Summary = "Buscar uauários pelo nome.", Description = "Endpoint responsável por localizar um registro de usuário através do nome.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersByNameAsync(string name)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(name)))
        using (LogContext.PushProperty("Metodo", "GetUsersByNameAsync"))
        {
            return await ExecuteAsync(nameof(GetUsersByNameAsync),
                  () => _userService.GetUsersByNameAsync(name), "Buscar usuários pelo nome");
        }
    }

    /// <summary>
    /// Endpoint responsável por atualizar os dados de um usuário.
    /// </summary>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    [HttpPut("update/user")]
    [CustomAuthorize(Claims.User, "Put")]
    [SwaggerOperation(Summary = "Atualizar uauário.", Description = "Endpoint responsável por atualizar os dados de um usuário.")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateRequest>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateRequest>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateRequest userUpdateRequest)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userUpdateRequest)))
        using (LogContext.PushProperty("Metodo", "UpdateAsync"))
        {
            return await ExecuteAsync(nameof(UpdateAsync),
                 () => _userService.UpdateAsync(userUpdateRequest), "Atualizar usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por recuperar os dados de um usuário atraves do Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/user/userid/{userId}")]
    [CustomAuthorize(Claims.User, "Get")]
    [SwaggerOperation(Summary = "Recuperar um usuário", Description = "Endpoint responsável por recuperar os dados de um usuário atraves do Id.")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(string userId)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userId)))
        using (LogContext.PushProperty("Metodo", "GetAsync"))
        {
            return await ExecuteAsync(nameof(GetAsync),
                 () => _userService.GetByIdAsync(Guid.Parse(userId)), "Recuperar um usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por ativar um usuário através do código de verificação.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("activate/user/code/{code}/userid/{userId}")]
    [SwaggerOperation(Summary = "Ativar usuário", Description = "Endpoint responsável por ativar um usuário através do código de verificação.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateAsync(string code, Guid userId)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { userId, code })))
        using (LogContext.PushProperty("Metodo", "ActivateAsync"))
        {
            return await ExecuteAsync(nameof(ActivateAsync),
                 () => _userService.ActivateUserAsync(userId, code), "Ativar usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por adicionar uma claim ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    [HttpPost("add/user/claim/username/{username}")]
    [CustomAuthorize(Claims.Claim, "Post")]
    [SwaggerOperation(Summary = "Remover claim do usuário", Description = "Endpoint responsável por adicionar uma claim ao usuário.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUserClaimAsync(string username, [FromBody] ClaimRequest claimRequest)
    {
        using (LogContext.PushProperty("Controller", "ClaimController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(claimRequest)))
        using (LogContext.PushProperty("Metodo", "AddUserClaimAsync"))
        {
            return await ExecuteAsync(nameof(AddUserClaimAsync),
                  () => _userService.AddUserClaimAsync(username, claimRequest), "Adicionar claim no usuário.");
        }
    }

    /// <summary>
    /// Endpoint responsável por remover uma claim ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    [HttpDelete("remove/user/claim/username/{username}")]
    [CustomAuthorize(Claims.Claim, "Delete")]
    [SwaggerOperation(Summary = "Remover claim do usuário", Description = "Endpoint responsável por remover uma claim ao usuário.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveUseClaimAsync([Required] string username, ClaimRequest claimRequest)
    {
        using (LogContext.PushProperty("Controller", "ClaimController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(claimRequest)))
        using (LogContext.PushProperty("Metodo", "RemoveUseClaimAsync"))
        {
            return await ExecuteAsync(nameof(RemoveUseClaimAsync),
                  () => _userService.RemoveUserClaimAsync(username, claimRequest), "Remover claim do usuário.");
        }
    }

    /// <summary>
    /// Endpoint responsável por recuperar dados da role de um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/user/roles/userid/{userId}")]
    [CustomAuthorize(Claims.Role, "Get")]
    [SwaggerOperation(Summary = "Recuperar roles do usuário", Description = "Endpoint responsável por recuperar dados da role de um usuário.")]
    [ProducesResponseType(typeof(ApiResponse<List<RolesResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserRolesAsync(Guid userId)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userId)))
        using (LogContext.PushProperty("Metodo", "GetUserRolesAsync"))
        {
            return await ExecuteAsync(nameof(GetUserRolesAsync),
                 () => _userService.GetUserRolesAsync(userId), "Recuperar roles do usuário.");
        }
    }

    /// <summary>
    /// Endpoint responsável por adicionar uma nova role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpPost("create/roles")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar role", Description = "Endpoint responsável por adicionar uma nova role.")]
    [ProducesResponseType(typeof(ApiResponse<RoleRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<RoleRequest>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ObjectResult> CreateRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "CreateRoleAsync"))
        {
            return await ExecuteAsync(nameof(CreateRoleAsync),
                 () => _userService.CreateRoleAsync(roleRequest), "Adicionar role.");
        }
    }

    /// <summary>
    /// Endpoint responsável por retornar os dados de todas as roles.
    /// </summary>
    /// <returns></returns>
    [HttpGet("get/roles")]
    [CustomAuthorize(Claims.Role, "Get")]
    [SwaggerOperation(Summary = "Recuperar todas as roles", Description = "Endpoint responsável por retornar os dados de todas as roles.")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ObjectResult> GetRolesAsync()
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Metodo", "GetRolesAsync"))
        {
            return await ExecuteAsync(nameof(GetRolesAsync),
                 () => _userService.GetRolesAsync(), "Recuperar todas as roles.");
        }
    }

    /// <summary>
    /// Endpoint responsável por adicionar uma lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpPost("add/claims/role")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar uma lista de claims na role", Description = "Endpoint responsável por adicionar uma lista de claims na role.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ObjectResult> AddClaimsToRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "AddClaimsToRole"))
        {
            return await ExecuteAsync(nameof(AddClaimsToRoleAsync),
                 () => _userService.AddClaimsToRoleAsync(roleRequest), "Adicionar claims em uma role.");
        }
    }

    /// <summary>
    /// Endpoint responsável por remover uma lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpDelete("remove/claims/role")]
    [CustomAuthorize(Claims.Role, "Delete")]
    [SwaggerOperation(Summary = "Remover uma lista de claims na role", Description = "Endpoint responsável por remover uma lista de claims na role.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ObjectResult> RemoverClaimsToRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "RemoverClaimToRole"))
        {
            return await ExecuteAsync(nameof(RemoverClaimsToRoleAsync),
                 () => _userService.RemoveClaimsToRoleAsync(roleRequest), "Remover claims em uma role.");
        }
    }

    /// <summary>
    /// Endpoint responsável por adicionar uma nova role ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpPost("add/role/user/username/{username}/rolename/{roleName}")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar role no usuário", Description = "Endpoint responsável por adicionar uma nova role ao usuário.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddRoleToUser(string username, string roleName)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { username, roleName })))
        using (LogContext.PushProperty("Metodo", "AddRoleToUser"))
        {
            return await ExecuteAsync(nameof(AddRoleToUser),
                 () => _userService.AddUserRoleAsync(new UserRoleRequest(username, roleName)), "Adicionar role no usuário.");
        }
    }

    /// <summary>
    /// Endpoint responsável por emover uma role do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpDelete("remove/user/{username}/role/{roleName}")]
    [CustomAuthorize(Claims.Role, "Delete")]
    [SwaggerOperation(Summary = "Remover role do usuário", Description = "Endpoint responsável por emover uma role do usuário.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveRoleToUser(string username, string roleName)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleName)))
        using (LogContext.PushProperty("Metodo", "RemoveRoleToUser"))
        {
            return await ExecuteAsync(nameof(RemoveRoleToUser),
                  () => _userService.RemoveUserRoleAsync(username, roleName), "Remover role do usuário.");
        }
    }
}
