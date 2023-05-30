using Mientreno.Compartido.Mensajes;

namespace Mientreno.Server.Service.Queue;

public interface IQueueProvider
{
	void Enqueue<T>(string queueName, T message) where T : Mensaje;
}