namespace Mientreno.Server.Connectors.Mailing;

public interface IMailSender
{
	Task SendMailAsync(string to, string name, string subject, string plainTextBody, string htmlBody, string? replyTo);
}
