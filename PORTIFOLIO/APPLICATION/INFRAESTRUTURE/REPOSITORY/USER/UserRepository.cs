using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.UTILS;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY.USER;

public class UserRepository : IUserRepository
{
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    public UserRepository(SignInManager<UserEntity> signInManager, 
        UserManager<UserEntity> userManager, 
        RoleManager<RoleEntity> roleManager)
    {
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
    public async Task<SignInResult> PasswordSignInAsync(UserEntity userEntity, string password, bool isPersistent, bool lockoutOnFailure)
        => await RetryPolicy.ExecuteAsync(
            () => _signInManager.PasswordSignInAsync(userEntity, password, isPersistent, lockoutOnFailure), 3);

    /// <summary>
    /// Método responsavel por criar um novo usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateUserAsync(UserEntity userEntity, string password)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.CreateAsync(userEntity, password), 3);

    /// <summary>
    /// Método responsavel por atualizar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateUserAsync(UserEntity userEntity)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.UpdateAsync(userEntity), 3);

    /// <summary>
    /// Método responsável por recuperar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserEntity> GetByAsync(Guid userId)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.FindByIdAsync(userId.ToString()), 3);

    /// <summary>
    /// Método responsável por recuperar vários usuários por id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserEntity>> GetByIdsAsync(List<Guid> userIds)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.Users.Where(user => userIds.Contains(user.Id)).ToListAsync(), 3);

    /// <summary>
    /// Método responsável por recuperar vários usuários por nome.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserEntity>> GetByNamesAsync(List<string> names)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.Users.Where(user =>
               names.Contains(user.FirstName)
            || names.Contains(user.LastName)
        ).ToListAsync(), 3);

    /// <summary>
    /// Método responsável por recuperar um usuário pelo username.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<UserEntity> GetWithUsernameAsync(string username)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.FindByNameAsync(username), 3);

    /// <summary>
    /// Método responsável por setar o nome de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetUserNameAsync(UserEntity userEntity, string username)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.SetUserNameAsync(userEntity, username), 3);

    /// <summary>
    /// Método responsável por mudar a senha do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="currentPassword"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(UserEntity userEntity, string currentPassword, string password)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.ChangePasswordAsync(userEntity, currentPassword, password), 3);

    /// <summary>
    /// Método responsável por setar o e-mail do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetEmailAsync(UserEntity userEntity, string email)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.SetEmailAsync(userEntity, email), 3);

    /// <summary>
    ///  Método responsável por setar o celular do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetPhoneNumberAsync(UserEntity userEntity, string phoneNumber)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.SetPhoneNumberAsync(userEntity, phoneNumber), 3);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ConfirmEmailAsync(UserEntity userEntity, string code)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.ConfirmEmailAsync(userEntity, code), 3);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<string> GenerateEmailConfirmationTokenAsync(UserEntity userEntity)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.GenerateEmailConfirmationTokenAsync(userEntity), 3);

    /// <summary>
    /// Método responsável por adicionar uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddClaimUserAsync(UserEntity userEntity, Claim claim)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.AddClaimAsync(userEntity, claim), 3);

    /// <summary>
    /// Método responsável por remover uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveClaimUserAsync(UserEntity userEntity, Claim claim)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.RemoveClaimAsync(userEntity, claim), 3);

    /// <summary>
    /// Método responsável por adicionar uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddToUserRoleAsync(UserEntity userEntity, string roleName)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.AddToRoleAsync(userEntity, roleName), 3);

    /// <summary>
    /// Método responsável por remover uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveToUserRoleAsync(UserEntity userEntity, string roleName)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.RemoveFromRoleAsync(userEntity, roleName), 3);

    /// <summary>
    /// Método responsável por recuperar as roles de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IList<string>> GetUserRolesAsync(UserEntity userEntity)
        => await RetryPolicy.ExecuteAsync(
            () => _userManager.GetRolesAsync(userEntity), 3);

    /// <summary>
    /// Método responsável por recuperar uma role.
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<RoleEntity> GetRoleAsync(string roleName)
        => await RetryPolicy.ExecuteAsync(
            () => _roleManager.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roleName)), 3);

    /// <summary>
    /// Método responsável por recuperar as claims de uma role.
    /// </summary>
    /// <param name="roleEntity"></param>
    /// <returns></returns>
    public async Task<IList<Claim>> GetRoleClaimsAsync(RoleEntity roleEntity)
        => await RetryPolicy.ExecuteAsync(
            () => _roleManager.GetClaimsAsync(roleEntity), 3);

    /// <summary>
    /// Método responsável por atualizar o token do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="providerName"></param>
    /// <param name="tokenName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task SetUserAuthenticationTokenAsync(UserEntity userEntity, string providerName, string tokenName, string token)
    {
        await _userManager
            .RemoveAuthenticationTokenAsync(userEntity, providerName, tokenName);

        await _userManager
            .SetAuthenticationTokenAsync(userEntity, providerName, tokenName, token);
    }
}
