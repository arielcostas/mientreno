using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Mailing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;

namespace QueueWorker
{
	public class MailQueueWorker : BackgroundService
	{
		private readonly IHub _hub;
		private readonly ILogger<MailQueueWorker> _logger;
		
		private readonly IConnection _connection;
		private readonly IMailSender _sender;
		private IModel? _channel;

		public MailQueueWorker(ILogger<MailQueueWorker> logger, IConnection rabbitConnection, IMailSender sender, IHub hub)
		{
			_connection = rabbitConnection;
			_logger = logger;
			_sender = sender;
			_hub = hub;
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

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += OnReceived;
			_channel.BasicConsume(Constantes.ColaEmails, false, consumer);
		}

		private async void OnReceived(object? sender, BasicDeliverEventArgs e)
		{
			var tx = _hub.StartTransaction("process-email", "send");

			var c1 = tx.StartChild("apply-template", "Apply email template");
			var bodyBytes = e.Body.ToArray();
			var email = Serializador.Deserializar<Email>(bodyBytes);
			
			var (subject, plain, html) = EmailTemplate.ApplyTemplate(email.Plantila, email.Idioma, email.Parametros);
			c1.Finish();

			var c2 = tx.StartChild("send-azure", "Send email through Azure");
			await _sender.SendMailAsync(email.DireccionDestinatario, email.NombreDestinatario, subject, plain, html);
			c2.Finish();

			_channel?.BasicAck(e.DeliveryTag, false);
			tx.Finish();
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