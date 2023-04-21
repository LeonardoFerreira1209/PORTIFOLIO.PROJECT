using APPLICATION.DOMAIN.CONTRACTS.ENTITY;

namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Criar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Atualizar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> DeleteAsync(T entity);

    /// <summary>
    /// Recuperar por id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetByIdAsync(Guid id);

    /// <summary>
    /// Recuperar por nome.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<T> GetByNameAsync(string name);

    /// <summary>
    /// Salvar mudanças.
    /// </summary>
    /// <returns></returns>
    Task SaveChangesAsync();
}
