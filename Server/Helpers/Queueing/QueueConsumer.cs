using Azure.Storage.Queues;
using Mientreno.Server.Queues;

namespace Mientreno.Server.Helpers.Queueing;

/// <summary>
/// Consume mensajes de una cola
/// </summary>
/// <typeparam name="T">El tipo de los mensajes</typeparam>
public abstract class QueueConsumer<T>
{
    protected readonly string QueueName;

    public QueueConsumer(string queueName)
    {
        QueueName = queueName;
    }

    protected abstract Task<T> GetMessageAsync();

    protected abstract Task DeleteMessageAsync(string messageId, string popReceipt);

    public delegate Task MessageHandler(T message);
}

public abstract class AzureQueueConsumer<T> : QueueConsumer<T>
{
    private readonly QueueClient _queue;
    private readonly QueueMessageSerializer<T> _serializer;

    public AzureQueueConsumer(string queueName, QueueServiceClient queueServiceClient) : base(queueName)
    {
        _serializer = new();

        _queue = queueServiceClient.GetQueueClient(queueName);
    }

    protected override Task DeleteMessageAsync(string messageId, string popReceipt)
    {
        return _queue.DeleteMessageAsync(messageId, popReceipt);
    }

    protected override async Task<T> GetMessageAsync()
    {
        var recibido = await _queue.ReceiveMessagesAsync(1);
        var mensaje = recibido.Value[0];

        return _serializer.Deserialize(mensaje.MessageText);
    }
}