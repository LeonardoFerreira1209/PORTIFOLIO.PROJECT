using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.BASE;

namespace APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.SENDGRID
{
    /// <summary>
    /// Classe de transporte de dados de reuquest base do SendGrid.
    /// </summary>
    public class SendGridMailRequestBase : MailRequestBase
    {
        /// <summary>
        /// Templates data.
        /// </summary>
        public object DynamicTemplateData { get; set; }
    }

    /// <summary>
    /// Record de transporte de dados do ASM.
    /// </summary>
    /// <param name="GroupId"></param>
    /// <param name="GroupToDisplay"></param>
    public record ASM(int GroupId, List<int> GroupToDisplay);

    /// <summary>
    /// Record de transporte de dados do MailSettings.
    /// </summary>
    /// <param name="BypassListManagement"></param>
    /// <param name="FooterSettings"></param>
    /// <param name="SandboxMode"></param>
    public record MailSettings(BypassListManagement BypassListManagement, FooterSettings FooterSettings, SandboxMode SandboxMode);

    /// <summary>
    /// Record de transporte de dados do BypassListManagement.
    /// </summary>
    /// <param name="Enable"></param>
    public record BypassListManagement(bool Enable = false);

    /// <summary>
    /// Record de transporte de dados do FooterSettings.
    /// </summary>
    /// <param name="Enable"></param>
    public record FooterSettings(bool Enable = false);

    /// <summary>
    /// Record de transporte de dados do SandboxMode.
    /// </summary>
    /// <param name="Enable"></param>
    public record SandboxMode(bool Enable = false);

    /// <summary>
    /// Record de transporte de dados do TrackingSettings.
    /// </summary>
    /// <param name="ClickTracking"></param>
    /// <param name="OpenTracking"></param>
    /// <param name="SubscriptionTracking"></param>
    public record TrackingSettings(ClickTracking ClickTracking, OpenTracking OpenTracking, SubscriptionTracking SubscriptionTracking);

    /// <summary>
    /// Record de transporte de dados do ClickTracking.
    /// </summary>
    /// <param name="Enable"></param>
    /// <param name="EnableText"></param>
    public record ClickTracking(bool Enable = true, bool EnableText = false);

    /// <summary>
    /// Record de transporte de dados do OpenTracking.
    /// </summary>
    /// <param name="Enable"></param>
    /// <param name="SubstitutionTag"></param>
    public record OpenTracking(bool Enable = true, string SubstitutionTag = "%open-track%");

    /// <summary>
    /// Record de transporte de dados do SubscriptionTracking.
    /// </summary>
    /// <param name="Enable"></param>
    public record SubscriptionTracking(bool Enable = false);

    /// <summary>
    /// Record de transporte de dados do Personalizations.
    /// </summary>
    /// <param name="Tos"></param>
    /// <param name="Ccs"></param>
    /// <param name="Bccs"></param>
    public record Personalizations(List<EmailAddress> Tos, List<EmailAddress> Ccs, List<EmailAddress> Bccs);
}
