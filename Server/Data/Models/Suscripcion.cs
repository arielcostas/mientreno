namespace Mientreno.Server.Data.Models;

public class Suscripcion
{
	public EstadoSuscripcion Estado { get; set; } = EstadoSuscripcion.NoSuscrito;
	public string CustomerId { get; set; } = string.Empty;
	public PlanSuscripcion? Plan { get; set; }
	public DateTime FechaInicio { get; set; }
	public DateTime FechaFin { get; set; }
	public bool RenovacionAutomatica { get; set; }
	
	public bool Operativa => Estado != EstadoSuscripcion.NoSuscrito && Plan != null && FechaFin > DateTime.Now;
}

public enum EstadoSuscripcion
{
	Activa,
	Expirada,
	Cancelada,
	NoSuscrito
}

public enum PlanSuscripcion
{
	Basic,
	Estandar,
	Prime
}