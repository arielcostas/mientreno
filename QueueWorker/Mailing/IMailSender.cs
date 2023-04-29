namespace Mientreno.Server.Helpers.Mailing;

public interface IMailSender
{
	void SendMail(string to, string name, string subject, string plainTextBody, string htmlBody);
	Task SendMailAsync(string to, string name, string subject, string plainTextBody, string htmlBody);
}
