using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.BASE;

namespace APPLICATION.DOMAIN.DTOS.REQUEST.MAIL;

/// <summary>
/// Dto de eventos de reenvio de e-mails.
/// </summary>
public class ResendMailEventDto
{
    /// <summary>
    /// Dto de data do evento de reenvio de e-mail de confirmação.
    /// </summary>
    public class ResendConirmationMailEventDto
    {
        /// <summary>
        /// E-mail e nome do remetente.
        /// </summary>
        public EmailAddress From { get; set; }

        /// <summary>
        /// E-mail e nome do destinatátio.
        /// </summary>
        public EmailAddress To { get; set; }

        /// <summary>
        /// Id do template do e-mail.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Dados do template do e-mail.
        /// </summary>
        public DynamicTemplateDataDto DynamicTemplateData { get; set; }

        /// <summary>
        /// Classe de dados do mapeamento do template do e-amil.
        /// </summary>
        public class DynamicTemplateDataDto
        {
            /// <summary>
            /// Nome do usuário.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Código de confirmação.
            /// </summary>
            public string Code { get; set; }
        };
    }
}
