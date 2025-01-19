using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Mientreno.Server.Configuration;
using MimeKit;

namespace Mientreno.Server.Connectors.Mailing;

public class MailkitMailSender(ILogger logger, IOptions<SmtpConfiguration> options) : IMailSender
{
    private readonly SmtpConfiguration _configuration = options.Value;

    public async Task SendMailAsync(string to, string name, string subject, string plainTextBody, string htmlBody,
        string? replyTo)
    {
        MimeMessage message = new();
        message.From.Add(new MailboxAddress("MiEntreno", _configuration.From));

        if (replyTo != null)
        {
            message.ReplyTo.Add(new MailboxAddress(name, replyTo));
        }

        message.To.Add(new MailboxAddress(name, to));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = plainTextBody,
            HtmlBody = htmlBody
        };

        message.Body = builder.ToMessageBody();

        using SmtpClient client = new();

        try
        {
            await client.ConnectAsync(_configuration.Host, _configuration.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration.Username, _configuration.Password);
            await client.SendAsync(message);
            logger.LogInformation("Mail sent to {Email} ({Name})", to, name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending mail to {Email} ({Name})", to, name);
            return;
        }
    }
}