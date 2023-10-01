using APPLICATION.DOMAIN.DTOS.RESPONSE.FILE;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Classe de extensão de arquivos.
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// Transforma entity em response.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static FileResponse ToResponse(this File file) => new()
        {
            Id = file.Id,
            Url = file.Url,
            ContentType = file.ContentType,
            Created = file.Created,
            Name = file.Name,
            Status = file.Status,
            Updated = file.Updated
        };
}
