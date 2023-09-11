using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

/// <summary>
/// Unidade de trabalho de banco de daods e controle de transações.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly Context _context;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="context"></param>
    public UnitOfWork(
        Context context)
    {
        _context = context;
    }

    /// <summary>
    /// Comita a transação.
    /// </summary>
    /// <returns></returns>
    public async Task CommitAsync()
        => await _context.SaveChangesAsync();

    /// <summary>
    /// Rollback a transação.
    /// </summary>
    /// <returns></returns>
    public async Task RollbackAsync()
        => await _context.DisposeAsync();
}
