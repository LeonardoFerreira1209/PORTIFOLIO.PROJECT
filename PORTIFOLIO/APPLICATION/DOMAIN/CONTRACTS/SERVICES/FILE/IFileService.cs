using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.FILE;

/// <summary>
/// Interface de Serviço de arquvios.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Método de upload de arquivos.
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    Task<BlobClient> UploadAsync(IFormFile formFile);

    /// <summary>
    /// étodo de upload de arquivo.
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task<BlobClient> GetBlobByName(string blobName);
}
