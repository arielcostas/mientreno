using System.Net.Http.Json;

namespace Mientreno.QueueWorker.Mailing;

public sealed class BrevoMailSender : IMailSender
{
	private readonly string From;
	private readonly ILogger Logger;

	public BrevoMailSender(ILogger<AzureCSMailSender> logger, string from)
	{
		Logger = logger;
		From = from;
	}
	
	public async Task SendMailAsync(string to, string name, string subject, string plainTextBody, string htmlBody,
		string? replyTo)
	{
		HttpClient client = new();

		var request = client.PostAsJsonAsync(
			"https://api.brevo.com/v3/smtp/email",
			new
			{
				sender = new
				{
					name = "Equipo de MiEntreno",
					email = From
				},
				to = new[] {
					new
					{
						Name = name,
						Email = to
					}
				},
				subject,
				htmlContent = htmlBody,
			}
		);

		client.Dispose();
	}
}