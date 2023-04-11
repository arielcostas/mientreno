using Mientreno.Server.Helpers.Mailing;

namespace Mientreno.Server.Helpers.Services;

public class Cartero
{
    private Queue<Email> _pendingEmails;

    private IMailSender _mailSender { get; set; }
    private ILogger<Cartero> _logger { get; set; }

    public Cartero(IMailSender sender, ILogger<Cartero> logger)
    {
        _pendingEmails = new Queue<Email>();
        _mailSender = sender;
        _logger = logger;
    }

    public void AddEmailToQueue(Email email)
    {
        _pendingEmails.Enqueue(email);
    }

    public void Run()
    {
        _logger.LogInformation("MailWorkerService is starting.");

        // Run while the service is not stopped
        while (true)
        {
            if (_pendingEmails.TryDequeue(out var email))
            {
                // Envía el email. Si hay varios, se ejecuta múltiples veces (pro el while)
                _logger.LogInformation("Sending email to {}", email.Address);
                _mailSender.SendMail(email.Address, email.Name, email.Subject, email.Plain, email.Html);
            }
            else
            {
                // Queue is empty, sleep 1 second and check again.
                Task.Delay(1000).Wait();
            }
        }
    }
}

public class Email
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Subject { get; set; }
    public string Plain { get; set; }
    public string Html { get; set; }

    public Email(string name, string address, string subject, string plain, string html)
    {
        Name = name;
        Address = address;
        Subject = subject;
        Plain = plain;
        Html = html;
    }
}
