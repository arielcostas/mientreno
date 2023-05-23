using System.Text;
using System.Text.Json;

namespace Mientreno.Compartido.Mensajes;

/// <summary>
/// Serializa y deserializa mensajes para enviarlos a la cola.
/// </summary>
public class Serializador
{
	public static byte[] Serializar(object o)
	{
		return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(o));
	}

	public static T? Deserializar<T>(byte[] data)
	{
		return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
	}
}
