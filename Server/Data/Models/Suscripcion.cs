namespace Mientreno.Server.Data.Models;

public class Suscripcion
{
	public EstadoSuscripcion Estado { get; set; } = EstadoSuscripcion.NoSuscrito;
	public PlanSuscripcion? Plan { get; set; } = null!;
	public DateTime FechaInicio { get; set; }
	public DateTime FechaFin { get; set; }
	public bool RenovacionAutomatica { get; set; }
}

public class PlanSuscripcion
{
	public int Id { get; set; }
	
	public string StripePrice { get; set; }
	
	public string Nombre { get; set; }
	public Modalidad Modalidad { get; set; }
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