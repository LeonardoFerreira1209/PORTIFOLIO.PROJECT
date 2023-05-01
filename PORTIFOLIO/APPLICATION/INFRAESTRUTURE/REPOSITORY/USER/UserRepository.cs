using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY.USER;

public class UserRepository : IUserRepository
{
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    public UserRepository(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager)
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
        => await _signInManager.PasswordSignInAsync(userEntity, password, isPersistent, lockoutOnFailure);

    /// <summary>
    /// Método responsavel por criar um novo usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateUserAsync(UserEntity userEntity, string password)
        => await _userManager.CreateAsync(userEntity, password);


    /// <summary>
    /// Método responsavel por atualizar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateUserAsync(UserEntity userEntity)
        => await _userManager.UpdateAsync(userEntity);

    /// <summary>
    /// Método responsável por recuperar um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserEntity> GetAsync(Guid userId)
        => await _userManager.FindByIdAsync(userId.ToString());

    /// <summary>
    /// Método responsável por recuperar um usuário pelo username.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<UserEntity> GetWithUsernameAsync(string username)
        => await _userManager.FindByNameAsync(username);

    /// <summary>
    /// Método responsável por setar o nome de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetUserNameAsync(UserEntity userEntity, string username)
        => await _userManager.SetUserNameAsync(userEntity, username);

    /// <summary>
    /// Método responsável por mudar a senha do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="currentPassword"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(UserEntity userEntity, string currentPassword, string password)
        => await _userManager.ChangePasswordAsync(userEntity, currentPassword, password);

    /// <summary>
    /// Método responsável por setar o e-mail do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetEmailAsync(UserEntity userEntity, string email)
        => await _userManager.SetEmailAsync(userEntity, email);

    /// <summary>
    ///  Método responsável por setar o celular do usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetPhoneNumberAsync(UserEntity userEntity, string phoneNumber)
        => await _userManager.SetPhoneNumberAsync(userEntity, phoneNumber);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ConfirmEmailAsync(UserEntity userEntity, string code)
        => await _userManager.ConfirmEmailAsync(userEntity, code);

    /// <summary>
    /// Método responsável por confirmar um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<string> GenerateEmailConfirmationTokenAsync(UserEntity userEntity)
        => await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);

    /// <summary>
    /// Método responsável por adicionar uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddClaimUserAsync(UserEntity userEntity, Claim claim)
        => await _userManager.AddClaimAsync(userEntity, claim);

    /// <summary>
    /// Método responsável por remover uma claim em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveClaimUserAsync(UserEntity userEntity, Claim claim)
        => await _userManager.RemoveClaimAsync(userEntity, claim);

    /// <summary>
    /// Método responsável por adicionar uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddToUserRoleAsync(UserEntity userEntity, string roleName)
        => await _userManager.AddToRoleAsync(userEntity, roleName);

    /// <summary>
    /// Método responsável por remover uma role em um usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RemoveToUserRoleAsync(UserEntity userEntity, string roleName)
        => await _userManager.RemoveFromRoleAsync(userEntity, roleName);

    /// <summary>
    /// Método responsável por recuperar as roles de usuário.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public async Task<IList<string>> GetUserRolesAsync(UserEntity userEntity)
        => await _userManager.GetRolesAsync(userEntity);

    /// <summary>
    /// Método responsável por recuperar uma role.
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<RoleEntity> GetRoleAsync(string roleName)
        => await _roleManager.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roleName));

    /// <summary>
    /// Método responsável por recuperar as claims de uma role.
    /// </summary>
    /// <param name="roleEntity"></param>
    /// <returns></returns>
    public async Task<IList<Claim>> GetRoleClaimsAsync(RoleEntity roleEntity)
        => await _roleManager.GetClaimsAsync(roleEntity);

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
