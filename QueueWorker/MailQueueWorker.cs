using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Mailing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace QueueWorker
{
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
			_logger.LogInformation("Starting...");

			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare("mientreno", ExchangeType.Direct, true, false);
			_channel.QueueDeclare(Constantes.ColaEmails, true, false, false);
			_channel.QueueBind(Constantes.ColaEmails, "mientreno", Constantes.ColaEmails);

			await base.StartAsync(cancellationToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("DONE??");
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += OnReceived;
			_channel.BasicConsume(Constantes.ColaEmails, false, consumer);

			if (consumer.IsRunning)
			{
				_logger.LogInformation("Consumer running");
			}

		}

		private async void OnReceived(object? sender, BasicDeliverEventArgs e)
		{
			var bodyBytes = e.Body.ToArray();
			var email = Serializador.Deserializar<Email>(bodyBytes);

			_logger.LogInformation($"Enviando email de tipo {email.Plantila} para {email.DireccionDestinatario}");

			var (subject, plain, html) = EmailTemplate.ApplyTemplate(email.Plantila, email.Idioma, email.Parametros);

			await _sender.SendMailAsync(email.DireccionDestinatario, email.NombreDestinatario, subject, plain, html);

			_logger.LogInformation($"Email enviado a {email.DireccionDestinatario}");

			_channel?.BasicAck(e.DeliveryTag, false);
		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stopping model...");
			_channel?.Close();
			_logger.LogInformation("Model stopped...");
			return base.StopAsync(cancellationToken);
		}
	}
}