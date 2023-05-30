namespace Mientreno.Compartido.Mensajes;

/// <summary>
/// Mensaje indicando que hay que enviar un email.
/// </summary>
public class NuevaFoto : Mensaje
{
	public string IdUsuario { get; set; }
	public string Nombre { get; set; }
	public string Apellidos { get; set; }
}