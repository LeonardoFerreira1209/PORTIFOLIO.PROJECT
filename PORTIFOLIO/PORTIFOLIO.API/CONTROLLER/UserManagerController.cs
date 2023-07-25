using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.ATTRIBUTE;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;
using System;
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
public class UserManagerController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="userService"></param>
    public UserManagerController(
        IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Método responsável por Ativar usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpGet("authetication")]
    [SwaggerOperation(Summary = "Autenticação do usuário", Description = "Método responsável por Autenticar usuário")]
    [ProducesResponseType(typeof(ApiResponse<TokenJWT>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AuthenticationAsync([FromHeader][Required] string username, [FromHeader][Required] string password)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { username, password })))
        using (LogContext.PushProperty("Metodo", "Authentication"))
        {
            return await Tracker.Time(()
                => _userService.AuthenticationAsync(new LoginRequest(username, password)), "Autenticar usuário");
        }
    }

    /// <summary>
    /// Método responsável por gerar um novo tokenJwt.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpGet("refreshtoken")]
    [SwaggerOperation(Summary = "Gerar token do usuário através de um refresh token", Description = "Método responsável por gerar um token de usuário através de um refresh token")]
    [ProducesResponseType(typeof(ApiResponse<TokenJWT>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status423Locked)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshTokenAsync([FromHeader][Required] string refreshToken)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(refreshToken)))
        using (LogContext.PushProperty("Metodo", "RefreshToken"))
        {
            return await Tracker.Time(()
                => _userService.RefreshTokenAsync(refreshToken), "Autenticar usuário");
        }
    }

    /// <summary>
    /// Método responsável por adicionar um usuario.
    /// </summary>
    /// <param name="userCreateRequest"></param>
    /// <returns></returns>
    [HttpPost("create/user")]
    [SwaggerOperation(Summary = "Criar uauário.", Description = "Método responsavel por criar usuário")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateRequest userCreateRequest)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userCreateRequest)))
        using (LogContext.PushProperty("Metodo", "Create"))
        {
            return await Tracker.Time(()
                => _userService.CreateAsync(userCreateRequest), "Criar usuário");
        }
    }

    /// <summary>
    /// Método responsável por atualizar um  usuario.
    /// </summary>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    [HttpPut("update/user")]
    [CustomAuthorize(Claims.User, "Put")]
    [SwaggerOperation(Summary = "Atualizar uauário.", Description = "Método responsavel por atualizar usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateRequest userUpdateRequest)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userUpdateRequest)))
        using (LogContext.PushProperty("Metodo", "Create"))
        {
            return await Tracker.Time(()
                => _userService.UpdateAsync(userUpdateRequest), "Atualizar usuário");
        }
    }

    /// <summary>
    /// Recuperar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/user/userid/{userId}")]
    [CustomAuthorize(Claims.User, "Get")]
    [SwaggerOperation(Summary = "Recuperar um usuário", Description = "Método responsável por Recuperar um usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(string userId)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userId)))
        using (LogContext.PushProperty("Metodo", "Get"))
        {
            return await Tracker.Time(()
                => _userService.GetByIdAsync(Guid.Parse(userId)), "Recuperar um usuário");
        }
    }

    /// <summary>
    /// Método responsável por Ativar usuário.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("activate/user/code/{code}/userid/{userId}")]
    [SwaggerOperation(Summary = "Ativar usuário", Description = "Método responsável por Ativar usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateAsync(string code, Guid userId)
    {
        using (LogContext.PushProperty("Controller", "UserController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { userId, code })))
        using (LogContext.PushProperty("Metodo", "activate"))
        {
            return await Tracker.Time(()
                => _userService.ActivateUserAsync(userId, code), "Ativar usuário");
        }
    }

    /// <summary>
    /// Método responsável por adicionar uma claim no usuário
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    [HttpPost("add/user/claim/username/{username}")]
    [CustomAuthorize(Claims.Claim, "Post")]
    [SwaggerOperation(Summary = "Remover claim do usuário", Description = "Método responsável por Adicionar claim no usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUserClaimAsync(string username, [FromBody] ClaimRequest claimRequest)
    {
        using (LogContext.PushProperty("Controller", "ClaimController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(claimRequest)))
        using (LogContext.PushProperty("Metodo", "AddClaim"))
        {
            return await Tracker.Time(()
                => _userService.AddUserClaimAsync(username, claimRequest), "Adicionar claim no usuário.");
        }
    }

    /// <summary>
    /// Método responsável por remover um claim do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    [HttpDelete("remove/user/claim/username/{username}")]
    [CustomAuthorize(Claims.Claim, "Delete")]
    [SwaggerOperation(Summary = "Remover claim do usuário", Description = "Método responsável por Remover claim do usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveUseClaimAsync([Required] string username, ClaimRequest claimRequest)
    {
        using (LogContext.PushProperty("Controller", "ClaimController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(claimRequest)))
        using (LogContext.PushProperty("Metodo", "RemoveClaim"))
        {
            return await Tracker.Time(()
                => _userService.RemoveUserClaimAsync(username, claimRequest), "Remover claim do usuário.");
        }
    }

    /// <summary>
    /// Método responsável por recuperar roles do usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/user/roles/userid/{userId}")]
    [CustomAuthorize(Claims.Role, "Get")]
    [SwaggerOperation(Summary = "Recuperar roles do usuário", Description = "Método responsável por recuperar roles do usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserRolesAsync(Guid userId)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userId)))
        using (LogContext.PushProperty("Metodo", "GetUserRoles"))
        {
            return await Tracker.Time(()
                => _userService.GetUserRolesAsync(userId), "Recuperar roles do usuário.");
        }
    }

    /// <summary>
    /// Método responsável por adicionar uma role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpPost("create/roles")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar role", Description = "Método responsável por Adicionar uma role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResponse<object>> CreateRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "AddRole"))
        {
            return await Tracker.Time(()
                => _userService.CreateRoleAsync(roleRequest), "Adicionar role.");
        }
    }

    /// <summary>
    /// Método responsável por recuperar todas as roles.
    /// </summary>
    /// <returns></returns>
    [HttpGet("get/roles")]
    [CustomAuthorize(Claims.Role, "Get")]
    [SwaggerOperation(Summary = "Recuperar todas as roles", Description = "Método responsável por recuperar todas as roles")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResponse<object>> GetRolesAsync()
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Metodo", "GetAll"))
        {
            return await Tracker.Time(()
                => _userService.GetRolesAsync(), "Recuperar todas as roles.");
        }
    }

    /// <summary>
    /// Método responsável por adicionar uma lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpPost("add/claims/role")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar uma lista de claims na role", Description = "Método responsável por adicionar uma lista de claims na role.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResponse<object>> AddClaimsToRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "AddClaimsToRole"))
        {
            return await Tracker.Time(()
                => _userService.AddClaimsToRoleAsync(roleRequest), "Adicionar claims em uma role.");
        }
    }

    /// <summary>
    ///  Método responsável por remover uma lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    [HttpDelete("remove/claims/role")]
    [CustomAuthorize(Claims.Role, "Delete")]
    [SwaggerOperation(Summary = "Remover uma lista de claims na role", Description = "Método responsável por Remover uma lista de claims na role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResponse<object>> RemoverClaimsToRoleAsync(RoleRequest roleRequest)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleRequest)))
        using (LogContext.PushProperty("Metodo", "RemoverClaimToRole"))
        {
            return await Tracker.Time(()
                => _userService.RemoveClaimsToRoleAsync(roleRequest), "Remover claims em uma role.");
        }
    }

    /// <summary>
    /// Adicionar uma role mo usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpPost("add/role/user/username/{username}/rolename/{roleName}")]
    [CustomAuthorize(Claims.Role, "Post")]
    [SwaggerOperation(Summary = "Adicionar role no usuário", Description = "Método responsável por Adicionar uma role no usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddRoleToUser(string username, string roleName)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(new { username, roleName })))
        using (LogContext.PushProperty("Metodo", "AddRoleToUser"))
        {
            return await Tracker.Time(()
                => _userService.AddUserRoleAsync(new UserRoleRequest(username, roleName)), "Adicionar role no usuário.");
        }
    }

    /// <summary>
    /// Remover a role do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpDelete("remove/user/{username}/role/{roleName}")]
    [CustomAuthorize(Claims.Role, "Delete")]
    [SwaggerOperation(Summary = "Remover role do usuário", Description = "Método responsável por Remover uma role do usuário")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveRoleToUser(string username, string roleName)
    {
        using (LogContext.PushProperty("Controller", "RoleController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(roleName)))
        using (LogContext.PushProperty("Metodo", "RemoveRoleToUser"))
        {
            return await Tracker.Time(()
                => _userService.RemoveUserRoleAsync(username, roleName), "Remover role do usuário.");
        }
    }
}
