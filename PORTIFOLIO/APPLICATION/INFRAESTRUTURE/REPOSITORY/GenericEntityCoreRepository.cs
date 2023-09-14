using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.ENTITY.BASE;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

/// <summary>
/// Repositório genérico com Entity.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericEntityCoreRepository<T> : IGenerictEntityCoreRepository<T> where T : Entity
{
    private readonly Context _context;

    private readonly LazyLoadingContext _lazyLoadingContext;

    /// <summary>
    /// Ctor
    /// </summary>
    public GenericEntityCoreRepository
        (Context context, LazyLoadingContext lazyLoadingContext)
    {
        _context = context;
        _lazyLoadingContext = lazyLoadingContext;
    }

    /// <summary>
    /// Criar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);

        return entity;
    }

    /// <summary>
    /// Criar vários.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public async Task<IList<T>> BulkInsertAsync(IList<T> entities)
    {
        await _context.BulkInsertAsync(entities);

        return entities;
    }

    /// <summary>
    /// Atualizar.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<T> UpdateAsync(T entity)
    {
        await _context.Set<T>().BatchUpdateAsync(entity);

        return entity;
    }

    /// <summary>
    /// Atualizar vários.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public async Task<IList<T>> BulkUpdateAsync(IList<T> entities)
    {
        await _context.BulkUpdateAsync(entities);

        return entities;
    }

    /// <summary>
    /// Deletar.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public async Task BulkDeleteAsync(IList<T> entities)
        => await _context.BulkDeleteAsync(entities);

    /// <summary>
    /// Recuperar por Id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="laziLoading"></param>
    /// <returns></returns>
    public Task<T> GetByIdAsync(Guid id, bool laziLoading)
        => laziLoading ? _lazyLoadingContext.Set<T>().FirstOrDefaultAsync(entity => entity.Id.Equals(id)) : _context.Set<T>().FirstOrDefaultAsync(entity => entity.Id.Equals(id));

    /// <summary>
    /// Recupera todos os registros do tipo T. Um predicado opcional pode ser fornecido para filtrar os registros.
    /// </summary>
    /// <param name="predicate">Um predicado opcional para filtrar os registros recuperados.</param>
    /// <returns>Uma tarefa que representa a operação de recuperação. O valor da tarefa é uma IQueryable<T> contendo todos os registros ou registros filtrados baseados no predicado.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(bool laziLoading, Expression<Func<T, bool>> predicate = null)
    {
        IQueryable<T> query = laziLoading ? _lazyLoadingContext.Set<T>() : _context.Set<T>();

        if (predicate != null) query = query.Where(predicate);

        return await query.ToListAsync();
    }


    /// <summary>
    /// Começar uma transação no banco de dados.
    /// </summary>
    /// <returns></returns>
    public async Task BeginTransactAsync()
        => await _context.Database.BeginTransactionAsync();

    /// <summary>
    /// Fechar uma conexão com o banco de dados.
    /// </summary>
    /// <returns></returns>
    public async Task CloseConnectionAsync()
        => await _context.Database.CloseConnectionAsync();

    /// <summary>
    /// Commitar e finalizar uma transação no banco de dados.
    /// </summary>
    /// <returns></returns>
    public async Task CommitTransactAsync()
        => await _context.Database.CommitTransactionAsync();

    /// <summary>
    /// Abrir uma conexão com o banco de dados.
    /// </summary>
    /// <returns></returns>
    public async Task OpenConnectAsync()
        => await _context.Database.OpenConnectionAsync();

    /// <summary>
    /// Resetar uma transação.
    /// </summary>
    /// <returns></returns>
    public async Task RollBackTransactionAsync()
        => await _context.Database.RollbackTransactionAsync();
}
