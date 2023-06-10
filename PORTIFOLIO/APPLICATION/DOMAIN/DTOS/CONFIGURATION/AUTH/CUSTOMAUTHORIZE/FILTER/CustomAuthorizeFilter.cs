using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;

namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.FILTER;

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
        var hasClaim = 
            context.HttpContext.User.Claims.Any(
                userClaim => _claims.Any(
                    claim => userClaim.Type.Equals(claim.Type) && userClaim.Value.Equals(claim.Value)));

        if (hasClaim is false) throw new UnauthorizedUserException(context.HttpContext.User);
    }
}
