using System.Text.Json;

namespace Mientreno.Server.Queues;

public sealed class QueueMessageSerializer<T>
{
    public string Serialize(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public T Deserialize(string json)
    {
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidCastException();
    }
}
