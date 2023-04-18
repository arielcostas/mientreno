using Azure.Storage.Queues;
using Mientreno.Server.Queues;

namespace Mientreno.Server.Helpers.Queueing;

/// <summary>
/// Envía mensajes a una cola
/// </summary>
/// <typeparam name="T">El tipo de los mensajes</typeparam>
public abstract class QueueProvider<T>
{
    protected readonly string QueueName;

    public QueueProvider(string queueName)
    {
        QueueName = queueName;
    }

    public void Submit(T message)
    {
        SubmitAsync(message).Wait();
    }

    public abstract Task SubmitAsync(T message);
}

public class AzureQueueProvider<T> : QueueProvider<T>
{
    private readonly QueueClient _queue;
    private readonly QueueMessageSerializer<T> _serializer;

    public AzureQueueProvider(string queueName, QueueServiceClient queueServiceClient) : base(queueName)
    {
        _serializer = new();

        _queue = queueServiceClient.GetQueueClient(queueName);
    }

    public override async Task SubmitAsync(T message)
    {
        string submission = _serializer.Serialize(message);
        await _queue.SendMessageAsync(submission);
    }
}