using Mientreno.Server.Helpers.Mailing;

namespace Mientreno.Server.Helpers.Services;

public class MailWorkerService : BackgroundService
{
    private Queue<Email> _pendingEmails;
    private IMailSender _mailSender;

    public MailWorkerService(IMailSender sender)
    {
        _pendingEmails = new Queue<Email>();
        _mailSender = sender;
    }

    public void AddEmailToQueue(Email email)
    {
        _pendingEmails.Enqueue(email);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run while the service is not stopped
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_pendingEmails.TryDequeue(out var email))
            {
                // Envía el email. Si hay varios, se ejecuta múltiples veces (pro el while)
                await _mailSender.SendMailAsync(email.To, email.Subject, email.Body);
            }
            else
            {
                // Queue is empty, sleep 1 second and check again.
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

public class Email
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public Email(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }
}
