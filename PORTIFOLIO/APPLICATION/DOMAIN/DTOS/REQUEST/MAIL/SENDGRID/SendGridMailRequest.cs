using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.BASE;

namespace APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.SENDGRID;

/// <summary>
/// Classe de transporte de request de e-mails do SendGrid.
/// </summary>
public class SendGridMailRequest : SendGridMailRequestBase
{
    /// <summary>
    /// Obtém ou define uma lista de mensagens e seus metadados. Cada objeto dentro das personalizações pode ser pensado como um envelope - ele define quem deve receber uma mensagem individual e como essa mensagem deve ser tratada. Para obter mais informações, consulte nossa documentação sobre Personalizações. Os parâmetros nas personalizações substituirão os parâmetros do mesmo nome no nível da mensagem.
    /// </summary>
    public List<Personalizations> Personalizations { get; set; }

    /// <summary>
    /// Obtém ou define uma lista de nomes de categoria para esta mensagem. Cada nome de categoria não pode exceder 255 caracteres. Você não pode ter mais de 10 categorias por solicitação.
    /// </summary>
    public List<string> Categories { get; set; }

    /// <summary>
    /// Obtém ou define uma lista de objetos de objetos de email contendo o endereço de email e o nome das pessoas que devem receber respostas ao seu email.
    /// </summary>
    public List<EmailAddress> ReplyTos { get; set; }

    /// <summary>
    /// Obtém ou define um carimbo de data/hora unix que permite especificar quando você deseja que seu email seja enviado do SendGrid. Isso não é necessário se você deseja que o e-mail seja enviado no momento de sua solicitação de API.
    /// </summary>
    public long? SendAt { get; set; }

    /// <summary>
    /// Obtém ou define uma ID que representa um lote de emails (também conhecidos como vários envios do mesmo email) a serem associados entre si para agendamento. Incluir um batch_id em sua solicitação permite que você inclua este e-mail nesse lote e também permite cancelar ou pausar a entrega de todo o lote. Para obter mais informações, leia sobre Cancelar envios agendados.
    /// </summary>
    public string BatchId { get; set; }

    /// <summary>
    /// Obtém ou define um objeto que permite especificar como lidar com cancelamentos de assinatura.
    /// </summary>
    public ASM Asm { get; set; }

    /// <summary>
    /// Obtém ou define o pool de IPs do qual você gostaria de enviar este e-mail.
    /// </summary>
    public string IpPoolName { get; set; }

    /// <summary>
    /// Obtém ou define uma coleção de diferentes configurações de email que você pode usar para especificar como deseja que esse email seja tratado.
    /// </summary>
    public MailSettings MailSettings { get; set; }

    /// <summary>
    /// Obtém ou define as configurações para determinar como você deseja rastrear as métricas de como seus destinatários interagem com seu email.
    /// </summary>
    public TrackingSettings TrackingSettings { get; set; }
}