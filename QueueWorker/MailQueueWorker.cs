using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.QueueWorker.Mailing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mientreno.QueueWorker;

public class MailQueueWorker : BackgroundService
{
	private readonly ILogger<MailQueueWorker> _logger;

	private readonly IConnection _connection;
	private readonly IMailSender _sender;
	private IModel? _channel;

	public MailQueueWorker(ILogger<MailQueueWorker> logger, IConnection rabbitConnection, IMailSender sender)
	{
		_connection = rabbitConnection;
		_logger = logger;
		_sender = sender;
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Starting email worker...");

		_channel = _connection.CreateModel();

		_channel.ExchangeDeclare("mientreno", ExchangeType.Direct, true, false);
		_channel.QueueDeclare(Constantes.ColaEmails, true, false, false);
		_channel.QueueBind(Constantes.ColaEmails, "mientreno", Constantes.ColaEmails);

		await base.StartAsync(cancellationToken);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return Task.Run(() =>
		{
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += OnReceived;
			_channel.BasicConsume(Constantes.ColaEmails, false, consumer);
		}, stoppingToken);
	}

	private async void OnReceived(object? sender, BasicDeliverEventArgs e)
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

		_channel?.BasicAck(e.DeliveryTag, false);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping mail worker...");
		_channel?.Close();
		_logger.LogInformation("Mail worker stopped...");
		return base.StopAsync(cancellationToken);
	}
}