namespace Mientreno.Server.Data.Models;

public class JornadaEntrenamiento
{
	public int Id { get; set; }
	
	public string Nombre { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
	public Alumno ClienteAsignado { get; set; } = new();
	public List<EjercicioProgramado> Ejercicios { get; set; } = new();
	
	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaRealizacion { get; set; }
	
	public int? Valoracion { get; set; }
	public string Comentario { get; set; } = string.Empty;
}

public class EjercicioProgramado
{
	public int Id { get; set; }
	public JornadaEntrenamiento Jornada { get; set; } = new();
	
	public Ejercicio Ejercicio { get; set; } = new();

	public int Series { get; set; }
	public int Repeticiones { get; set; }
}