using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.BASE;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

/// <summary>
/// File Repository
/// </summary>
public class FileRepository : GenericEntityCoreRepository<File>, IFileRepository
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="context"></param>
    public FileRepository(Context context) : base(context) { }
}
