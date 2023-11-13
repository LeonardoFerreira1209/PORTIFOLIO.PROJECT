using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES;

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
    Task<File> UploadAsync(Guid userId, IFormFile formFile);

    /// <summary>
    /// étodo de upload de arquivo.
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task<BlobClient> GetBlobByNameAsync(string blobName);

    /// <summary>
    /// Método responsável por deletar um blob pelon nome.
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task DeleteBlobByNameAsync(string blobName);
}
