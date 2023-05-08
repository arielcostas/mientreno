namespace Mientreno.Server.Models;

#pragma warning disable CS8618

/// <summary>
/// Un cuestionario se realiza entre un entrenador y un alumno. Realizándose periódicamente, permiten
/// comprobar la evolución de los alumnos y la eficacia de los entrenamientos.
/// </summary>
public class Cuestionario
{
	public Guid Id { get; set; }
	public Alumno Alumno { get; set; }
	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaFormalizacion { get; set; }

	public byte Edad { get; set; }
	public byte AlturaCm { get; set; }
	public float MasaKilogramos { get; set; }
	public byte FrecuenciaCardiacaReposo { get; set; }

	public Habitos Habitos { get; set; }
	public Perimetros Perimetros { get; set; }

	public Cuestionario()
	{
		Id = Guid.NewGuid();
		FechaCreacion = DateTime.Now;
	}
}

public class Perimetros
{
	public byte PectoralInspiracion { get; set; }
	public byte PectoralExpiracion { get; set; }
	public byte Abd1 { get; set; } // TODO: Nombre completo
	public byte AbdominalOmbligo { get; set; }
	public byte Cintura { get; set; }
	public byte Cadera { get; set; }
	public byte CuadricepsMaximo { get; set; }
	public byte CuadricepsMinimo { get; set; }
	public byte Gastrocnemio { get; set; }
}