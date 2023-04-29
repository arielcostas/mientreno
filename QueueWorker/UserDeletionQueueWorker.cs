namespace QueueWorker
{
	public class UserDeletionQueueWorker : BackgroundService
	{
		private readonly ILogger<UserDeletionQueueWorker> _logger;

		public UserDeletionQueueWorker(ILogger<UserDeletionQueueWorker> logger)
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