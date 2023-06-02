namespace Mientreno.Server.Data.Models;

public class Suscripcion
{
	public EstadoSuscripcion Estado { get; set; } = EstadoSuscripcion.NoSuscrito;
	public string CustomerId { get; set; } = string.Empty;
	public string Plan { get; set; } = null!;
	public DateTime FechaInicio { get; set; }
	public DateTime FechaFin { get; set; }
	public bool RenovacionAutomatica { get; set; }
}

public enum EstadoSuscripcion
{
	Activa,
	Expirada,
	Cancelada,
	NoSuscrito
}

public enum Modalidad
{
	Mensual,
	Anual
}