using APPLICATION.DOMAIN.ENTITY.ENTITY;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.ENTITY
{
    public class Events : Entity
    {
        /// <summary>
        /// Nome do evento.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descição do evento.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// _retry private
        /// </summary>
        private bool _retry;

        /// <summary>
        /// Retentar.
        /// </summary>
        public bool Retry
        {
            get => _retry;

            private set
                => _retry = value && Retries < 3;
        }

        /// <summary>
        /// Retentativas.
        /// </summary>
        public int Retries { get; set; }

        /// <summary>
        /// Status do evento.
        /// </summary>
        public new EventStatus Status { get; set; }
    }
}
