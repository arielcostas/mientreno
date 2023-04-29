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

	public void SendMail(
		string toAddress,
		string toName,
		string subject,
		string plainBody,
		string htmlBody
	)
	{
		SendMailAsync(toAddress, toName, subject, plainBody, htmlBody).Wait();
	}

	public async Task SendMailAsync(
		string toAddress,
		string toName,
		string subject,
		string plainBody,
		string htmlBody
	)
	{
		var recipientAddress = new EmailAddress(toAddress, toName);
		EmailRecipients recipients = new EmailRecipients();
		recipients.To.Add(recipientAddress);

		EmailMessage options = new(
			From,
			recipients,
			new(subject)
			{
				PlainText = plainBody,
				Html = htmlBody
			}
		);

		try
		{
			var operation = await Client.SendAsync(WaitUntil.Completed, options);
			Logger.LogInformation("Successfully sent email to {} -- messageId: {}", toAddress, operation.Id);
		}
		catch (RequestFailedException ex)
		{
			Logger.LogError("Failed to send email to {}: {} - {}", toAddress, ex.ErrorCode, ex.Message);
		}
	}
}
