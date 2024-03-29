﻿using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.BASE;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

/// <summary>
/// Repositório do usuário.
/// </summary>
public class UserRepository : BaseRepository, IUserRepository
{
    private readonly Context _context;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UserRepository(IOptions<AppSettings> options, SignInManager<User> signInManager,
        Context context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager) : base(options)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Retorna o resultado de autenticação do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="password"></param>
    /// <param name="isPersistent"></param>
    /// <param name="lockoutOnFailure"></param>
    /// <returns></returns>
    public async Task<SignInResult> PasswordSignInAsync(User userEntity, string password, bool isPersistent, bool lockoutOnFailure)
        => await _signInManager.PasswordSignInAsync(userEntity, password, isPersistent, lockoutOnFailure);

    /// <summary>
    /// Método responsavel por criar um novo usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateUserAsync(User userEntity, string password)
        => await _userManager.CreateAsync(userEntity, password);

    /// <summary>
    /// Método responsavel por atualizar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateUserAsync(User userEntity)
        => await _userManager.UpdateAsync(userEntity);

    /// <summary>
    /// Método responsável por recuperar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<User> GetByIdAsync(Guid userId)
        => await _userManager.Users
                    .Include(user => user.File).FirstAsync(user => user.Id.Equals(userId));

    /// <summary>
    /// Método responsável por recuperar vários usuários por id.
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetByIdsAsync(List<Guid> userIds)
        => await _userManager.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();

    /// <summary>
    /// Método responsável por recuperar vários usuários por nome.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetByNamesAsync(List<string> names)
    {
        var normalizedNames
            = names.ConvertAll(name
                => name.RemoveAccentAndConvertToLower());

        var users = await _context.Users.ToListAsync();

        return users.Where(
            user => normalizedNames.Any(
                normalized =>
                    $"{user.FirstName.RemoveAccentAndConvertToLower()} {user.LastName.RemoveAccentAndConvertToLower()}".Contains(normalized)
                    ||
                normalized.Contains(
                    $"{user.FirstName.RemoveAccentAndConvertToLower()} {user.LastName.RemoveAccentAndConvertToLower()}")
                )
            ||
            names.Any(name => user.Email.Contains(name) || user.UserName.Contains(name)
            ||
            name.Contains(user.Email) || name.Contains(user.UserName))
        ).ToList();
    }

    /// <summary>
    /// Método responsável por recuperar um usuário pelo username.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<User> GetWithUsernameAsync(string username)
        => await _userManager.FindByNameAsync(username);

    /// <summary>
    /// Método responsável por setar o nome de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetUserNameAsync(User userEntity, string username)
        => await _userManager.SetUserNameAsync(userEntity, username);

    /// <summary>
    /// Método responsável por mudar a senha do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="currentPassword"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(User userEntity, string currentPassword, string password)
        => await _userManager.ChangePasswordAsync(userEntity, currentPassword, password);

    /// <summary>
    /// Método responsável por setar o e-mail do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetEmailAsync(User userEntity, string email)
        => await _userManager.SetEmailAsync(userEntity, email);

    /// <summary>
    ///  Método responsável por setar o celular do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetPhoneNumberAsync(User userEntity, string phoneNumber)
        => await _userManager.SetPhoneNumberAsync(userEntity, phoneNumber);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ConfirmEmailAsync(User userEntity, string code)
        => await _userManager.ConfirmEmailAsync(userEntity, code);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<string> GenerateEmailConfirmationTokenAsync(User userEntity)
        => await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);

    /// <summary>
    /// Método responsável por adicionar uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddClaimUserAsync(User userEntity, Claim claim)
        => await _userManager.AddClaimAsync(userEntity, claim);

    /// <summary>
    /// Método responsável por remover uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveClaimUserAsync(User userEntity, Claim claim)
        => await _userManager.RemoveClaimAsync(userEntity, claim);

    /// <summary>
    /// Método responsável por adicionar uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddToUserRoleAsync(User userEntity, string roleName)
        => await _userManager.AddToRoleAsync(userEntity, roleName);

    /// <summary>
    /// Método responsável por remover uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveToUserRoleAsync(User userEntity, string roleName)
        => await _userManager.RemoveFromRoleAsync(userEntity, roleName);

    /// <summary>
    /// Método responsável por recuperar as roles de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IList<string>> GetUserRolesAsync(User userEntity)
        => await _userManager.GetRolesAsync(userEntity);

    /// <summary>
    /// Método responsável por recuperar uma role.
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<Role> GetRoleAsync(string roleName)
        => await _roleManager.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roleName));

    /// <summary>
    /// Método responsável por recuperar as claims de uma role.
    /// </summary>
    /// <param name="roleEntity"></param>
    /// <returns></returns>
    public async Task<IList<Claim>> GetRoleClaimsAsync(Role roleEntity)
        => await _roleManager.GetClaimsAsync(roleEntity);

    /// <summary>
    /// Método responsável por atualizar o token do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="providerName"></param>
    /// <param name="tokenName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task SetUserAuthenticationTokenAsync(
        User userEntity, string providerName, string tokenName, string token)
    {
        await _userManager
            .RemoveAuthenticationTokenAsync(
                userEntity, providerName, tokenName).ContinueWith(
                    async (identityResultTask) =>
                    {
                        await _userManager
                            .SetAuthenticationTokenAsync(
                                userEntity, providerName, tokenName, token);
                    });
    }

    /// <summary>
    /// Método responsavel por gravar um código de confirmação de usuário.
    /// </summary>
    /// <param name="userCodeEntity"></param>
    /// <returns></returns>
    public async Task<UserCode> AddUserConfirmationCode(
        UserCode userCodeEntity)
    {
        await _context.AddAsync(userCodeEntity);

        return userCodeEntity;
    }

    /// <summary>
    /// Método responsavel por atualizar um código de confirmação de usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public UserCode UpdateUserConfirmationCode(UserCode userCodeEntity)
    {
        _context.Update(userCodeEntity);

        return userCodeEntity;
    }

    /// <summary>
    ///  Método responsavel por obter os dados de confirmação de usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<UserCode> GetUserConfirmationCode(
        Guid userId, string code)
        => await _context.AspNetUserCodes.FirstOrDefaultAsync(
                x => x.UserId.Equals(userId) && x.NumberCode.Equals(code));

    /// <summary>
    /// Método responsável por verificar se o cpf já existe em um uwuário.
    /// </summary>
    /// <param name="cpf"></param>
    /// <returns></returns>
    public async Task<bool> IsCpfAlreadyRegistered(string cpf)
        => await _context.Users.AnyAsync(user => user.CPF.Equals(cpf));
}
