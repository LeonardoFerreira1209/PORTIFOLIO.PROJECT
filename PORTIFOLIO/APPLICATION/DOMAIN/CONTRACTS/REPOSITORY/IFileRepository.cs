using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;

/// <summary>
/// File Repository interface. 
/// </summary>
public interface IFileRepository : IGenerictEntityCoreRepository<File> { }
