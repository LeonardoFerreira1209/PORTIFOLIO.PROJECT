namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;

public interface IUnitOfWork
{
    /// <summary>
    /// Commita a transação.
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    /// Reverte a transação.
    /// </summary>
    /// <returns></returns>
    Task RollbackAsync();
}
