using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE;

public class FileResponse
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public virtual Status Status { get; set; }

    /// <summary>
    /// Url de acesso do arquivo.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Nome do arquivo.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Tipo do conteudo.
    /// </summary>
    public string ContentType { get; set; }
}
