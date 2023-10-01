using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.FILTER;
using APPLICATION.DOMAIN.ENUMS;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.ATTRIBUTE;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Atributo de autorização customizavel.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="values"></param>
    public CustomAuthorizeAttribute(Claims claim, params string[] values) : base(typeof(CustomAuthorizeFilter))
    {
        Arguments = new object[] { 
            values.Select(value => new Claim(claim.ToString(), value)).ToList() 
        };
    }
}
