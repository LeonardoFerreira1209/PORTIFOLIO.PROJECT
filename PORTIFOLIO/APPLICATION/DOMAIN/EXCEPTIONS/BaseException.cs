using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;

namespace APPLICATION.DOMAIN.EXCEPTIONS;

public abstract class BaseException : Exception
{
    public BaseException() {

    }

    public ErrorResponse Response { get; set; }
}
