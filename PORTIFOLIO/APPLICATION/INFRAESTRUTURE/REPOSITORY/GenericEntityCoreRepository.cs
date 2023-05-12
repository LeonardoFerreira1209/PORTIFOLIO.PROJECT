using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.ENTITY.ENTITY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY
{
    public class GenericEntityCoreRepository<T> : IGenerictEntityCoreRepository<T> where T : Entity
    {
        private readonly Context _context;

        /// <summary>
        /// Ctor
        /// </summary>
        public GenericEntityCoreRepository
            (Context context) {
                _context = context;
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
        /// <returns></returns>
        public Task<T> GetByIdAsync(Guid id)
            => _context.Set<T>().FirstOrDefaultAsync(entity => entity.Id.Equals(id));

        /// <summary>
        /// Recuperar todos.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

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

        /// <summary>
        /// Salvar mudanças.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
