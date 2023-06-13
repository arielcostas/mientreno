using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mientreno.Server.Connectors.Queue;

public class RabbitQueueConsumer<T> : IQueueConsumer<T> where T : Mensaje
{
	private readonly IModel _channel;
	
	public RabbitQueueConsumer(IConnection connection)
	{
		_channel = connection.CreateModel();
	}
	
	public void Listen(string queueName, Action<T> callback)
	{
		_channel.ExchangeDeclare("mientreno", ExchangeType.Direct, true, false);
		_channel.QueueDeclare(queueName, true, false, false);
		_channel.QueueBind(queueName, "mientreno", queueName);
		
		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += (sender, e) =>
		{
			var bodyBytes = e.Body.ToArray();
			var data = Serializador.Deserializar<T>(bodyBytes);
			
			if (data is null) return;
			
			callback(data);
			
			_channel.BasicAck(e.DeliveryTag, false);
		};
		
		_channel.BasicConsume(queueName, false, consumer);
	}
}