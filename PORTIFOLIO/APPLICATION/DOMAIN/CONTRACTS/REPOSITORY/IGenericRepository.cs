using APPLICATION.DOMAIN.ENTITY.BASE;
using System.Linq.Expressions;

namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;

/// <summary>
/// Repositório genérico.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGenericRepository<T> where T : Entity
{
    /// <summary>
    /// Criar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Criar vários.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<IList<T>> BulkInsertAsync(IList<T> entities);

    /// <summary>
    /// Atualizar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Atualizar vários.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<IList<T>> BulkUpdateAsync(IList<T> entities);

    /// <summary>
    /// Deletar vários.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task BulkDeleteAsync(IList<T> entities);

    /// <summary>
    /// Recuperar por id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetByIdAsync(Guid id, bool lazyLoading);

    /// <summary>
    /// Recuperar todos.
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<T>> GetAllAsync(bool lazyLoading, Expression<Func<T, bool>> predicate);
}
