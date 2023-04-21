namespace QueueWorker
{
    public class MailQueueWorker : BackgroundService
    {
        private readonly ILogger<MailQueueWorker> _logger;

        public MailQueueWorker(ILogger<MailQueueWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}