namespace Mientreno.Server.Workers;

public abstract class AbstractWorker : BackgroundService
{
	protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
}