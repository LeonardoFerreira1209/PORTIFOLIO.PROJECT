using APPLICATION.ENUMS;

namespace APPLICATION.DOMAIN.CONTRACTS.ENTITY
{
    public interface IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; set; }

    }
}
