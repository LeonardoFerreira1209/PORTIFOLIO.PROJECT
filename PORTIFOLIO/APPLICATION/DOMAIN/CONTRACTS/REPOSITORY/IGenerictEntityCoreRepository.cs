using APPLICATION.DOMAIN.ENTITY.ENTITY;

namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;

public interface IGenerictEntityCoreRepository<T> : IGenericRepository<T> where T : Entity
{
    /// <summary>
    /// Começar transação.
    /// </summary>
    /// <returns></returns>
    Task BeginTransactAsync();

    /// <summary>
    /// Finalizar transação.
    /// </summary>
    /// <returns></returns>
    Task CommitTransactAsync();

    /// <summary>
    /// Resetar transação.
    /// </summary>
    /// <returns></returns>
    Task RollBackTransactionAsync();

    /// <summary>
    /// Abrir uma conexão com o banco de dados.
    /// </summary>
    /// <returns></returns>
    Task OpenConnectAsync();

    /// <summary>
    /// Fechar uma conexão com o banco de dados.
    /// </summary>
    /// <returns></returns>
    Task CloseConnectionAsync();
}
