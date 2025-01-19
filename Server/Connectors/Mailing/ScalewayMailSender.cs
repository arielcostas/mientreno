namespace Mientreno.Server.Connectors.Mailing;

public sealed class ScalewayMailSender : IMailSender
{
	private readonly ILogger _logger;
	private readonly string _from;
	private readonly string _secretKey;
	private readonly string _projectId;

	public ScalewayMailSender(ILogger logger, string from, string secretKey, string projectId)
	{
		_logger = logger;
		_from = from;
		_secretKey = secretKey;
		_projectId = projectId;
	}
	
	public async Task SendMailAsync(string to, string name, string subject, string plainTextBody, string htmlBody,
		string? replyTo)
	{
		_logger.LogInformation("Sending mail to {Email} ({Name})", to, name);
		HttpClient client = new();
		client.DefaultRequestHeaders.Add("X-Auth-Token", _secretKey);
		var request = await client.PostAsJsonAsync(
			"https://api.scaleway.com/transactional-email/v1alpha1/regions/fr-par/emails",
			new
			{
				from = new
				{
					name = "Equipo de MiEntreno",
					email = replyTo ?? _from
				},
				to = new[] {
					new
					{
						Name = name,
						Email = to
					}
				},
				subject,
				text = plainTextBody,
				html = htmlBody,
				project_id = _projectId
			}
		);
		
		if (!request.IsSuccessStatusCode)
		{
			_logger.LogError("Error sending mail to {Email} ({Name}): {StatusCode} {ReasonPhrase}", to, name,
				request.StatusCode, request.ReasonPhrase);
			return;
		}
		
		_logger.LogInformation("Mail sent to {Email} ({Name})", to, name);
		client.Dispose();
	}
}