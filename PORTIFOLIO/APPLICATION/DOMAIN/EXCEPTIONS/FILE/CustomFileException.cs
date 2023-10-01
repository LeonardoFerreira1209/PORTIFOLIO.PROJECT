using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.EXCEPTIONS.BASE;
using System.Net;

namespace APPLICATION.DOMAIN.EXCEPTIONS.FILE;

/// <summary>
/// Exceptions
/// </summary>
public class CustomFileException
{
    /// <summary>
    /// Exception de arquivo existente no blob.
    /// </summary>
    public class ConflictFileException : BaseException
    {
        public ConflictFileException(
            object dados)
        {
            Response = new ErrorResponse
               (HttpStatusCode.Conflict, dados, new List<DadosNotificacao>() {
                   new DadosNotificacao("O arquivo já  existe no blob!"),
               });
        }
    }

    public class NotFoundFileException<T> : BaseException
    {
        public NotFoundFileException(
            T dados)
        {
            Response = new ErrorResponse
               (HttpStatusCode.NotFound, dados, new List<DadosNotificacao>() {
                   new DadosNotificacao("O arquivo não foi encontrado no storage!"),
               });
        }
    }
}
