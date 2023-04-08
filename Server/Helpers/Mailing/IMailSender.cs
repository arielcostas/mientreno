namespace Mientreno.Server.Helpers.Mailing;

public interface IMailSender
{
    void SendMail(string to, string subject, string body);
    Task SendMailAsync(string to, string subject, string body);
}
