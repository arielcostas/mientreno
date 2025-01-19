using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Connectors.Mailing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mientreno.Server.Workers;

public class MailQueueWorker : BackgroundService
{
	private readonly ILogger<MailQueueWorker> _logger;

	private readonly IConnection _connection;
	private readonly IMailSender _sender;
	private IChannel? _channel;

	public MailQueueWorker(ILogger<MailQueueWorker> logger, IConnection rabbitConnection, IMailSender sender)
	{
		_connection = rabbitConnection;
		_logger = logger;
		_sender = sender;
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Starting email worker...");

		_channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

		await _channel.ExchangeDeclareAsync("mientreno", ExchangeType.Direct, true, false);
		await _channel.QueueDeclareAsync(Constantes.ColaEmails, true, false, false);
		await _channel.QueueBindAsync(Constantes.ColaEmails, "mientreno", Constantes.ColaEmails);

		await base.StartAsync(cancellationToken);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_channel == null) throw new Exception("Channel not created");

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += OnReceived;
		return _channel.BasicConsumeAsync(Constantes.ColaEmails, false, consumer, stoppingToken);
	}

	private async Task OnReceived(object? sender, BasicDeliverEventArgs e)
	{
		var bodyBytes = e.Body.ToArray();
		var email = Serializador.Deserializar<Email>(bodyBytes);

		if (email == null)
		{
			throw new Exception("Mensaje inválido");
		}
		
		var (subject, plain, html) = EmailTemplate.ApplyTemplate(email.Plantila, email.Idioma, email.Parametros);

		await _sender.SendMailAsync(email.DireccionDestinatario, email.NombreDestinatario, subject, plain, html,
			email.ResponderA);

		_channel?.BasicAckAsync(e.DeliveryTag, false);
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping mail worker...");
		
		if (_channel is null) return;
		await _channel.CloseAsync(cancellationToken: cancellationToken);
		await base.StopAsync(cancellationToken);
		
		_logger.LogInformation("Mail worker stopped...");
	}
}