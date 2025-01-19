using Mientreno.Compartido.Mensajes;

namespace Mientreno.Server.Connectors.Queue;

public interface IQueueConsumer<T> where T : Mensaje
{
	Task Listen(string queueName, Action<T> callback);
}