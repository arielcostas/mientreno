using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Mientreno.Server.Helpers.Queue;

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