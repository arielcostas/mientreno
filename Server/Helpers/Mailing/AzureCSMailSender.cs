using Azure;
using Azure.Communication.Email;

namespace Mientreno.Server.Helpers.Mailing;

public sealed class AzureCSMailSender : IMailSender
{
    private readonly EmailClient Client;
    private readonly string From;
    private readonly ILogger Logger;

    public AzureCSMailSender(ILogger<AzureCSMailSender> logger, string connectionString, string from)
    {
        Logger = logger;
        Client = new(connectionString);
        From = from;
    }

    public void SendMail(string to, string subject, string body)
    {
        SendMailAsync(to, subject, body).RunSynchronously();
    }

    public async Task SendMailAsync(string to, string subject, string body)
    {
        EmailMessage options = new(
            From,
            to,
            new(subject)
            {
                PlainText = body
            }
        );

        try
        {
            var operation = await Client.SendAsync(WaitUntil.Completed, options);
            Logger.LogInformation($"Successfully sent email to {to}: messageId{operation.Id}");
        }
        catch (RequestFailedException ex)
        {
            Logger.LogError($"Failed to send email to {to}: {ex.ErrorCode} - {ex.Message}");
        }
    }
}
