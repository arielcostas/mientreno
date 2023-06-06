using Azure;
using Azure.Communication.Email;

namespace Mientreno.QueueWorker.Mailing;

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

	public async Task SendMailAsync(
		string toAddress,
		string toName,
		string subject,
		string plainBody,
		string htmlBody,
		string? replyTo
	)
	{
		var recipientAddress = new EmailAddress(toAddress, toName);
		EmailRecipients recipients = new EmailRecipients();
		recipients.To.Add(recipientAddress);

		EmailMessage options = new(
			From,
			recipients,
			new EmailContent(subject)
			{
				PlainText = plainBody,
				Html = htmlBody
			}
		);

		if (!string.IsNullOrWhiteSpace(replyTo))
		{
			options.ReplyTo.Add(new EmailAddress(replyTo));
		}

		try
		{
			var operation = await Client.SendAsync(WaitUntil.Started, options);
			Logger.LogInformation("Successfully sent email to {} -- messageId: {}", toAddress, operation.Id);
		}
		catch (RequestFailedException ex)
		{
			Logger.LogError("Failed to send email to {}: {} - {}", toAddress, ex.ErrorCode, ex.Message);
		}
	}
}
