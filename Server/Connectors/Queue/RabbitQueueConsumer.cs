using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mientreno.Server.Connectors.Queue;

public class RabbitQueueConsumer<T> : IQueueConsumer<T> where T : Mensaje
{
	private readonly IConnection _connection;
	private IChannel? _channel = null;
	
	public RabbitQueueConsumer(IConnection connection)
	{
		_connection = connection;
		
	}
	
	public async Task Listen(string queueName, Action<T> callback)
	{
		if (_channel == null) _channel = await _connection.CreateChannelAsync();
		
		await _channel.ExchangeDeclareAsync("mientreno", ExchangeType.Direct, true, false);
		await _channel.QueueDeclareAsync(queueName, true, false, false);
		await _channel.QueueBindAsync(queueName, "mientreno", queueName);
		
		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (sender, e) =>
		{
			var bodyBytes = e.Body.ToArray();
			var data = Serializador.Deserializar<T>(bodyBytes);
			
			if (data is null) return;
			
			callback(data);
			
			await _channel.BasicAckAsync(e.DeliveryTag, false);
		};
		
		await _channel.BasicConsumeAsync(queueName, false, consumer);
	}
}