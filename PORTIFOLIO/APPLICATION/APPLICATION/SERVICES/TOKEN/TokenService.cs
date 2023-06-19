using APPLICATION.DOMAIN.BUILDERS.TOKEN;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.TOKEN;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Security.Claims;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;

namespace APPLICATION.APPLICATION.SERVICES.TOKEN
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;
        private readonly IOptions<AppSettings> _appsettings;

        public TokenService(UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager, IOptions<AppSettings> appsettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appsettings = appsettings;
        }

        /// <summary>
        /// Cria o JWT TOKEN
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundUserException"></exception>
        public async Task<TokenJWT> CreateJsonWebToken(string username)
        {
            Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(TokenService)} - METHOD {nameof(CreateJsonWebToken)}\n");

            var userEntity = await User(username) ?? throw new NotFoundUserException(username);

            var roles = await Roles(userEntity);

            var claims = await Claims(userEntity, roles);

            Log.Information($"[LOG INFORMATION] - Criando o token do usuário.\n");

            return await Task.FromResult(new TokenJwtBuilder()
               .AddUsername(username)
                .AddSecurityKey(JwtSecurityKey.Create(_appsettings.Value.Auth.SecurityKey))
                    .AddSubject("HYPER.IO PROJECTS L.T.D.A")
                        .AddIssuer(_appsettings.Value.Auth.ValidIssuer)
                            .AddAudience(_appsettings.Value.Auth.ValidAudience)
                                .AddExpiry(_appsettings.Value.Auth.ExpiresIn)
                                    .AddRoles(roles)
                                        .AddClaims(claims)
                                            .Builder(userEntity));
        }

        /// <summary>
        /// Return de User.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<UserEntity> User(string username)
            => await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));

        /// <summary>
        /// Return de Roles.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<Claim>> Roles(UserEntity user)
        {
            return await _userManager.GetRolesAsync(user).ContinueWith(rolesTask =>
            {
                var roles = rolesTask.Result;

                return roles.AsParallel().Select(role => new Claim("role", role)).ToList();
            });
        }

        /// <summary>
        /// Return de Claims.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<Claim>> Claims(UserEntity user, IList<Claim> roles)
        {
            var claims = new List<Claim>();

            claims.AddRange(await _userManager.GetClaimsAsync(user));

            var rolesName = roles.AsParallel().Select(role => role.Value).ToList();

            if (roles is not null && roles.Any())
            {
                await _roleManager.Roles.Where(role 
                    => rolesName.Contains(role.Name)).ToListAsync().ContinueWith(rolesTask =>
                {
                    var roles = rolesTask.Result;

                    roles.AsParallel().ForAll(role 
                        => claims.AddRange(_roleManager.GetClaimsAsync(role).Result));
                });
            }

            return claims.Distinct().ToList();
        }
    }
}
