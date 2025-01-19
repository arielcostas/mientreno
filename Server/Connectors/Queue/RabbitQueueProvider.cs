using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;

namespace Mientreno.Server.Connectors.Queue;

public class RabbitQueueProvider : IQueueProvider
{
	private readonly IConnection _connection;
	private IChannel? _channel = null;

	public RabbitQueueProvider(IConnection connection)
	{
		_connection = connection;
	}

	public async Task Enqueue<T>(string queueName, T message) where T : Mensaje
	{
		if (_channel == null) _channel = await _connection.CreateChannelAsync();
		
		await _channel.QueueDeclareAsync(queueName, true, false, false);

		var body = Serializador.Serializar(message);

		await _channel.BasicPublishAsync("", queueName, true, body);
	}
}