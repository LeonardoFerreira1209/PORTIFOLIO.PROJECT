using System.Net;

namespace APPLICATION.DOMAIN.EXCEPTIONS.USER;

public class InvalidCreateUserRequestException<T> : Exception where T : class
{
    public InvalidCreateUserRequestException() {
        
    }

    public const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

    public string Reason { get; set; } = string.Empty;
    public T Object { get; set; }
}
