using APPLICATION.DOMAIN.ENTITY.BASE;

namespace APPLICATION.DOMAIN.ENTITY.FILE;

/// <summary>
/// Entidade de arquivos.
/// </summary>
public class File : Entity
{
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
