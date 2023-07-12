namespace APPLICATION.DOMAIN.DTOS.MAIL.BASE;

/// <summary>
/// Classe base de transporte de dados do Email
/// </summary>
public abstract class MailRequestBase
{
    /// <summary>
    /// Obtém ou define um objeto de email contendo o endereço de email e o nome do remetente. A codificação Unicode não é suportada para o campo from.
    /// </summary>
    public EmailAddress From { get; set; }

    /// <summary>
    /// Obtém ou define um objeto de email que contém o endereço de email e o nome da pessoa que deve receber respostas ao seu email.
    /// </summary>
    public EmailAddress ReplyTo { get; set; }

    /// <summary>
    /// Obtém ou define o assunto do seu e-mail.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Obtém ou define uma lista na qual você pode especificar o conteúdo do seu e-mail. Você pode incluir vários tipos de conteúdo mime, mas deve especificar pelo menos um. Para incluir mais de um tipo mime, basta adicionar outro objeto ao array contendo os parâmetros de tipo e valor. Se incluídos, text/plain e text/html devem ser os primeiros índices do array nesta ordem. Se você optar por incluir os tipos mime text/plain ou text/html, eles devem ser os primeiros índices da matriz de conteúdo na ordem text/plain, text/html.*O conteúdo NÃO é obrigatório se você estiver usando um modelo transacional e tiver definiu o template_id no Pedido.
    /// </summary>
    public List<Content> Contents { get; set; }

    /// <summary>
    /// Obtém ou define uma lista de objetos na qual você pode especificar quaisquer anexos que deseja incluir.
    /// </summary>
    public List<Attachments> Attachments { get; set; }

    /// <summary>
    /// Obtém ou define a id de um modelo que você gostaria de usar. Se você usar um modelo que contenha conteúdo e um assunto (texto ou html), não precisará especificá-los nas respectivas personalizações ou parâmetros de nível de mensagem.
    /// </summary>
    public string TemplateId { get; set; }
}

/// <summary>
/// Record de transporte de dados de EmailAddress.
/// </summary>
/// <param name="Name"></param>
/// <param name="Email"></param>
public record EmailAddress(string Name, string Email);

/// <summary>
/// Record de transporte de dados de Content.
/// </summary>
/// <param name="Type"></param>
/// <param name="Value"></param>
public record Content(string Type, string Value);

/// <summary>
///  Record de transporte de dados de Attachments.
/// </summary>
/// <param name="Content"></param>
/// <param name="Filename"></param>
/// <param name="Type"></param>
/// <param name="Disposition"></param>
public record Attachments(string Content, string Filename, string Type, string Disposition);
