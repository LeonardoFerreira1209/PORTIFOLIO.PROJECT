using System.IdentityModel.Tokens.Jwt;

namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;

/// <summary>
/// Classe referente ao Token
/// </summary>
public class TokenJWT
{
    /// <summary>
    /// Token de segurança
    /// </summary>
    private readonly JwtSecurityToken token;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="token"></param>
    internal TokenJWT(JwtSecurityToken token) => this.token = token;

    /// <summary>
    /// Validade do token
    /// </summary>
    public DateTime ValidTo => token.ValidTo;

    /// <summary>
    /// Valor do token.
    /// </summary>
    public string Value => new JwtSecurityTokenHandler().WriteToken(this.token);
}

