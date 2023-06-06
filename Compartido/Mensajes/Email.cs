namespace Mientreno.Compartido.Mensajes;

/// <summary>
/// Mensaje indicando que hay que enviar un email.
/// </summary>
public class Email : Mensaje
{
	public string Plantila { get; set; }
	public string Idioma { get; set; } = "default";
	public string[] Parametros { get; set; }
	public string NombreDestinatario { get; set; }
	public string DireccionDestinatario { get; set; }
	
	public string? ResponderA { get; set; }
}