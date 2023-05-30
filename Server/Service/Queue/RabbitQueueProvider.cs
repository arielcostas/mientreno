using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;

namespace Mientreno.Server.Service.Queue;

public class RabbitQueueProvider : IQueueProvider
{
	private readonly IModel _channel;

	public RabbitQueueProvider(IConnection connection)
	{
		_channel = connection.CreateModel();
	}

	public void Enqueue<T>(string queueName, T message) where T : Mensaje
	{
		_channel.QueueDeclare(queueName, true, false, false, null);

		var body = Serializador.Serializar(message);

		_channel.BasicPublish("", queueName, null, body);
	}
}