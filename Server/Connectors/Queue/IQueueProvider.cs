using Mientreno.Compartido.Mensajes;

namespace Mientreno.Server.Connectors.Queue;

public interface IQueueProvider
{
	Task Enqueue<T>(string queueName, T message) where T : Mensaje;
}