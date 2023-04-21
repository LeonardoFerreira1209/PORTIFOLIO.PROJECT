using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY
{
    public abstract class GenericEntityCoreRepository<T> : IGenerictEntityCoreRepository<T> where T : class
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected GenericEntityCoreRepository() { }

        /// <summary>
        /// Criar.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> CreateAsync(T entity)
        {
            using (var context = new Context())

            await context.Set<T>().AddAsync(entity);

            return entity;
        }

        /// <summary>
        /// Atualizar.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletar.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<T> DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recuperar por Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<T> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recuperar por nome.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<T> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Começar uma transação no banco de dados.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task BeginTransactAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fechar uma conexão com o banco de dados.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task CloseConnectionAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Commitar e finalizar uma transação no banco de dados.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task CommitTransactAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abrir uma conexão com o banco de dados.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task OpenConnectAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resetar uma transação.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task RollBackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Salvar mudanças.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
