using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST;

namespace APPLICATION.DOMAIN.DTOS.MAIL;

/// <summary>
/// Classe de conversão de dados do e-mail de confirmação.
/// </summary>
public class EventMailDetailsConfirmEmailDto
{
    /// <summary>
    /// Remetente
    /// </summary>
    public EmailAddress From { get; set; }

    /// <summary>
    /// Destinatario.
    /// </summary>
    public EmailAddress To { get; set; }

    /// <summary>
    /// Id do template.
    /// </summary>
    public string TemplateId { get; set; }

    /// <summary>
    /// Dados dinamicos do template de e-ail de confirmação.
    /// </summary>
    public DynamicTemplateConfirmEmailDataDto DynamicTemplateData { get; set; }
}

/// <summary>
/// Clase de dados dinamicos.
/// </summary>
public class DynamicTemplateConfirmEmailDataDto
{
    /// <summary>
    /// Nome do destinatario.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Código de confirmação.
    /// </summary>
    public string Code { get; set; }
}
