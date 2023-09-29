using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;

namespace APPLICATION.DOMAIN.EXCEPTIONS.BASE;

/// <summary>
/// Classe base de exceptions tratados.
/// </summary>
public abstract class BaseException : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    public BaseException() { }

    /// <summary>
    /// Response dos exceptiopns.
    /// </summary>
    public ErrorResponse Response { get; set; }
}
