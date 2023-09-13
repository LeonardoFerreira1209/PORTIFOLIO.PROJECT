using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;

namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.FILTER;

/// <summary>
/// Classe custom de autenticação de usuários.
/// </summary>
public class CustomAuthorizeFilter : IAuthorizationFilter
{
    private readonly List<Claim> _claims;

    /// <summary>
    /// Filtro de autorização customizavel.
    /// </summary>
    /// <param name="claims"></param>
    public CustomAuthorizeFilter(List<Claim> claims) => _claims = claims;

    /// <summary>
    /// Autorização customizavel.
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthenticated =
            context.HttpContext.User.Identity.IsAuthenticated;

        var claimsPrincipal =
            context.HttpContext.User;

        if ((isAuthenticated && HasClaims(claimsPrincipal)) is false) throw new UnauthorizedUserException(null);
    }

    /// <summary>
    /// Verifica se o usuário têm as claims necessárias.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    private bool HasClaims(ClaimsPrincipal claimsPrincipal)
    {
        var hasClaim = _claims.Any(claim
            => claimsPrincipal.HasClaim(claim.Type, claim.Value));

        return hasClaim;
    }
}
