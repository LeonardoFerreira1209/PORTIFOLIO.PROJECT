﻿using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;

public interface IUserService
{
    /// <summary>
    /// Método responsável por fazer a autenticação do usuário
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> AuthenticationAsync(LoginRequest loginRequest);

    /// <summary>
    /// Recuperar usuário através do Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ObjectResult> GetAsync(Guid userId);

    /// <summary>
    /// Método responsável por criar um novo usuário.
    /// </summary>
    /// <param name="userCreateRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> CreateAsync(UserCreateRequest userCreateRequest);

    /// <summary>
    /// Método responsável por atualizar um usuário.
    /// </summary>
    /// <param name="userRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> UpdateAsync(UserUpdateRequest userUpdateRequest);

    /// <summary>
    /// Método responsável por atualizar a imagem de um usuario.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="formFile"></param>
    /// <returns></returns>
    Task<ObjectResult> UpdateUserIamgeAsync(Guid id, IFormFile formFile);

    /// <summary>
    /// Método responsavel por ativar um usuário.
    /// </summary>
    /// <param name="activateUserRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> ActivateAsync(ActivateUserRequest activateUserRequest);

    /// <summary>
    /// Método responsavel por adicionar uma claim ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> AddClaimAsync(string username, ClaimRequest claimRequest);

    /// <summary>
    /// Método responsavel por remover uma claim do usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="claimRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> RemoveClaimAsync(string username, ClaimRequest claimRequest);

    /// <summary>
    /// Método responsavel por adicionar uma role ao usuário.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<ObjectResult> AddRoleAsync(string username, string roleName);

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
    Task<ObjectResult> RemoveRoleAsync(string username, string roleName);
}
