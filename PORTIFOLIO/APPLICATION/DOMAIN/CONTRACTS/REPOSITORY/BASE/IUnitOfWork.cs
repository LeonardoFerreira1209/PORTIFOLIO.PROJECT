namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;

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
