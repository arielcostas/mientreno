using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace QueueWorker
{
    public class MailQueueWorker : BackgroundService
    {
        private readonly ILogger<MailQueueWorker> _logger;
        private readonly IConnection _connection;

        private IModel? _channel;

        public MailQueueWorker(ILogger<MailQueueWorker> logger, IConnection rabbitConnection)
        {
            _connection = rabbitConnection;
            _logger = logger;
            
            _logger.LogInformation("Prepared");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("mientreno", ExchangeType.Direct, true, false);
            _channel.QueueDeclare("email", true, false, false);
            _channel.QueueBind("email", "mientreno", "email");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceived;
            if (consumer.IsRunning)
            {
                _logger.LogInformation("Consumer running");
            }

            _channel.BasicConsume("email", false, consumer);
        }

        private void OnReceived(object? sender, BasicDeliverEventArgs e)
        {
            var bodyBytes = e.Body.ToArray();
            var bodyString = Encoding.Default.GetString(bodyBytes);
            Console.WriteLine(bodyString);
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