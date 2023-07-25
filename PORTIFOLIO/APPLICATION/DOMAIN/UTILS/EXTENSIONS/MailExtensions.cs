using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using SendGrid.Helpers.Mail;
using ASM = SendGrid.Helpers.Mail.ASM;
using ClickTracking = SendGrid.Helpers.Mail.ClickTracking;
using Content = SendGrid.Helpers.Mail.Content;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using OpenTracking = SendGrid.Helpers.Mail.OpenTracking;
using SubscriptionTracking = SendGrid.Helpers.Mail.SubscriptionTracking;
using TrackingSettings = SendGrid.Helpers.Mail.TrackingSettings;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Classe de extensão de e-mails.
/// </summary>
public static class MailExtensions
{
    public static EmailAddress ToSendGridEmailAddress(this DTOS.MAIL.REQUEST.EmailAddress emailAddress)
        => new(emailAddress.Email, emailAddress.Name);

    /// <summary>
    /// Converte SendGridMailRequest para SendGridMessage.
    /// </summary>
    /// <param name="sendGridMailRequest"></param>
    /// <returns></returns>
    public static SendGridMessage ToSendGridMessage(this SendGridMailRequest sendGridMailRequest)
    {
        return new SendGridMessage
        {
            Asm = sendGridMailRequest.Asm is not null ? new ASM
            {
                GroupId = sendGridMailRequest.Asm.GroupId,
                GroupsToDisplay = sendGridMailRequest.Asm.GroupToDisplay

            } : null,

            Attachments = sendGridMailRequest?.Attachments?.Select(attach =>
            {
                return new Attachment
                {
                    Content = attach?.Content,
                    Disposition = attach?.Disposition,
                    Filename = attach?.Filename,
                    Type = attach?.Type
                };

            }).ToList(),

            From = sendGridMailRequest.From.ToSendGridEmailAddress(),
            Categories = sendGridMailRequest?.Categories,

            Contents = sendGridMailRequest?.Contents?.Select(content =>
            {
                return new Content
                {
                    Type = content?.Type,
                    Value = content?.Value,
                };

            }).ToList(),

            Subject = sendGridMailRequest?.Subject,
            Personalizations = sendGridMailRequest?.Personalizations?.Select(personalization =>
            {
                return new Personalization
                {
                    Bccs = personalization?.Bccs?.Select(bcc =>
                    {
                        return bcc.ToSendGridEmailAddress();

                    }).ToList(),

                    Ccs = personalization?.Ccs?.Select(ccs =>
                    {
                        return ccs.ToSendGridEmailAddress();

                    }).ToList(),

                    Tos = personalization?.Tos?.Select(tos =>
                    {
                        return tos.ToSendGridEmailAddress();

                    }).ToList(),
                };

            }).ToList(),

            BatchId = sendGridMailRequest?.BatchId,
            IpPoolName = sendGridMailRequest?.IpPoolName,

            ReplyTo = sendGridMailRequest.ReplyTo is not null ? new EmailAddress
            {
                Email = sendGridMailRequest.ReplyTo?.Email,
                Name = sendGridMailRequest.ReplyTo?.Name

            } : null,

            SendAt = sendGridMailRequest?.SendAt,
            TrackingSettings = sendGridMailRequest.TrackingSettings is not null ? new TrackingSettings
            {
                ClickTracking = sendGridMailRequest.TrackingSettings?.ClickTracking is not null ? new ClickTracking
                {
                    Enable = sendGridMailRequest.TrackingSettings.ClickTracking.Enable,
                    EnableText = sendGridMailRequest.TrackingSettings.ClickTracking.EnableText,

                } : null,

                OpenTracking = sendGridMailRequest.TrackingSettings?.OpenTracking is not null ? new OpenTracking
                {
                    Enable = sendGridMailRequest.TrackingSettings.OpenTracking.Enable,
                    SubstitutionTag = sendGridMailRequest.TrackingSettings.OpenTracking.SubstitutionTag

                } : null,

                SubscriptionTracking = sendGridMailRequest.TrackingSettings?.SubscriptionTracking is not null ? new SubscriptionTracking
                {
                    Enable = sendGridMailRequest.TrackingSettings.SubscriptionTracking.Enable

                } : null,

            } : null,
        };
    }
}
