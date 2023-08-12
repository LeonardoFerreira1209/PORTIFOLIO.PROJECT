using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using Microsoft.AspNetCore.Mvc;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;

/// <summary>
/// Interface de UserService
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Método responsável por fazer a autenticação do usuário
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> AuthenticationAsync(LoginRequest loginRequest);

    /// <summary>
    /// Método responsável por gerar um novo tokenJwt.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<ObjectResult> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Recuperar usuário através do Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ObjectResult> GetByIdAsync(Guid userId);

    /// <summary>
    /// Método responsável por criar um novo usuário.
    /// </summary>
    /// <param name="userCreateRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> CreateAsync(UserCreateRequest userCreateRequest);

    /// <summary>
    ///  Método responsável por atualizar um usuário.
    /// </summary>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> UpdateAsync(UserUpdateRequest userUpdateRequest);

    /// <summary>
    /// Método responsavel por ativar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<ObjectResult> ActivateUserAsync(Guid userId, string code);

    /// <summary>
    /// Método responsavel por adicionar uma claim ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> AddUserClaimAsync(string username, ClaimRequest claimRequest);

    /// <summary>
    /// Método responsavel por remover uma claim do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> RemoveUserClaimAsync(string username, ClaimRequest claimRequest);

    /// <summary>
    /// Método responsavel por adicionar uma role ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<ObjectResult> AddUserRoleAsync(UserRoleRequest userRoleRequest);

    /// <summary>
    /// Método responsavel por recuperar roles do usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ObjectResult> GetUserRolesAsync(Guid userId);

    /// <summary>
    /// Método responsavel por remover uma role do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<ObjectResult> RemoveUserRoleAsync(string username, string roleName);

    /// <summary>
    /// Método responsável por criar uma nova role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> CreateRoleAsync(RoleRequest roleRequest);

    /// <summary>
    /// Método responsavel por retornar todas as roles.
    /// </summary>
    /// <returns></returns>
    Task<ObjectResult> GetRolesAsync();

    /// <summary>
    /// Método responsável por adicionar uma nova lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> AddClaimsToRoleAsync(RoleRequest roleRequest);

    /// <summary>
    /// Método responsavel por remover uma lista de claims na role.
    /// </summary>
    /// <param name="roleRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> RemoveClaimsToRoleAsync(RoleRequest roleRequest);
}
