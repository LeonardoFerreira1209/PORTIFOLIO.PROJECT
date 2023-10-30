using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.ENUMS;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using static APPLICATION.DOMAIN.EXCEPTIONS.CustomFileException;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.APPLICATION.SERVICES.FILE;

/// <summary>
/// Serviço de Arquivos
/// </summary>
public class FileService : IFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileRepository _fileRepository;
    private readonly BlobContainerClient _blobContainerClient;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="appsettings"></param>
    public FileService(
        IOptions<AppSettings> appsettings, IUnitOfWork unitOfWork, IFileRepository fileRepository)
    {
        _unitOfWork = unitOfWork;
        _fileRepository = fileRepository;
        _blobContainerClient = new BlobContainerClient(
            appsettings.Value.ConnectionStrings.AzureBlobStorage, appsettings.Value.AzureBlobStorage.Container);
    }

    /// <summary>
    /// Método de upload de arquivo.
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    /// <exception cref="ConflictFileException"></exception>
    public async Task<File> UploadAsync(Guid userId, IFormFile formFile)
    {
        Log.Information(
           $"[LOG INFORMATION] - SET TITLE {nameof(FileService)} - METHOD {nameof(UploadAsync)}\n");

        try
        {
            var fileName 
                = $"{userId}_{formFile.FileName}";

            var blobStorage
                = _blobContainerClient.GetBlobClient(fileName);

            return await blobStorage.ExistsAsync().ContinueWith(
                async (taskResult) =>
                {
                    var azureResponse =
                            taskResult.Result;

                    if(await blobStorage.ExistsAsync()) await blobStorage.DeleteAsync();

                    await blobStorage.UploadAsync(
                        formFile.OpenReadStream());

                    return await _fileRepository.CreateAsync(new File
                    {
                        ContentType = formFile.ContentType,
                        Created = DateTime.UtcNow,
                        Name = formFile.Name,
                        Status = Status.Active,
                        Url = blobStorage.Uri.ToString()

                    }).ContinueWith(async (taskResult) =>
                    {
                        var file = taskResult.Result;

                        await _unitOfWork.CommitAsync();

                        Log.Information(
                         $"[LOG INFORMATION] - Arquivo enviado para o blob com sucesso {fileName}\n");

                        return file;

                    }).Unwrap();

                }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Metodo que recupera os dados de um blob pelo nome.
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    public async Task<BlobClient> GetBlobByNameAsync(string blobName)
    {
        Log.Information(
           $"[LOG INFORMATION] - SET TITLE {nameof(FileService)} - METHOD {nameof(GetBlobByNameAsync)}\n");

        try
        {
            var blobStorage
                = _blobContainerClient.GetBlobClient(blobName);

            return await blobStorage.ExistsAsync().ContinueWith(
                taskResult =>
            {
                if (taskResult.Result.Value is false)
                    throw new NotFoundFileException<string>(blobName);

                Log.Information(
                     $"[LOG INFORMATION] - Blob recuperado com sucesso {JsonConvert.SerializeObject(blobStorage)}\n");

                return blobStorage;
            });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por deletar um blob pelon nome.
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    public async Task DeleteBlobByNameAsync(string blobName)
    {
        Log.Information(
           $"[LOG INFORMATION] - SET TITLE {nameof(FileService)} - METHOD {nameof(DeleteBlobByNameAsync)}\n");

        try
        {
            await _blobContainerClient.DeleteBlobIfExistsAsync(blobName).ContinueWith(
                taskResult =>
                {
                    Log.Information(
                     $"[LOG INFORMATION] - Blob {blobName} deletado com sucesso!\n");
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }
}
