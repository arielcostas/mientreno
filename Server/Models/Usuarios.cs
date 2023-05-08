using System.ComponentModel.DataAnnotations;

namespace Mientreno.Server.Models;

#pragma warning disable CS8618 // Initialized by EF Core

/// <summary>
/// Un usuario genérico, con su ID, datos básicos y credenciales.
/// </summary>
public class Usuario
{
	[Key] public Guid Id { get; set; }
	[EmailAddress] public string Email { get; set; }
	public string Nombre { get; set; }
	public string Apellidos { get; set; }

	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaEliminacion { get; set; }

	public Credenciales Credenciales { get; set; }
	public List<Sesion> Sesiones { get; set; }

	public string? CodigoVerificacionEmail { get; set; }
	public bool EmailVerificado { get; set; } = false;

	public Usuario()
	{
		Id = Guid.NewGuid();
		FechaCreacion = DateTime.Now;
		Nombre = string.Empty;
		Apellidos = string.Empty;
		Credenciales = new Credenciales();
	}
}

/// <summary>
/// Un entrenador es un usuario que tiene alumnos a los que les asigna rutinas, objetivos, etc.
/// </summary>
public class Entrenador : Usuario
{
	public List<Alumno> Alumnos { get; set; } = new();

	public List<Ejercicio> Ejercicios { get; set; } = new();
	public List<Categoria> Categorias { get; set; } = new();

	public Entrenador()
	{
	}

	public Entrenador(Usuario u)
	{
		Id = u.Id;
		Nombre = u.Nombre;
		
		Email = u.Email;
		EmailVerificado = u.EmailVerificado;
		CodigoVerificacionEmail = u.CodigoVerificacionEmail;
		
		Apellidos = u.Apellidos;
		FechaCreacion = u.FechaCreacion;
		FechaEliminacion = u.FechaEliminacion;
		Credenciales = u.Credenciales;
	}
}

/// <summary>
/// Un alumno es un usuario que recibe entrenamiento de un entrenador.
/// </summary>
public class Alumno : Usuario
{
	public Entrenador Entrenador { get; set; }
	public List<Cuestionario> Cuestionarios { get; set; } = new();

	public Alumno()
	{
	}

	public Alumno(Usuario u)
	{
		Id = u.Id;
		Nombre = u.Nombre;
		Apellidos = u.Apellidos;
		FechaCreacion = u.FechaCreacion;
		FechaEliminacion = u.FechaEliminacion;
		Credenciales = u.Credenciales;
	}
}

/// <summary>
/// Credenciales para un usuario o entrenador.
/// </summary>
public class Credenciales
{
	public string Contraseña { get; set; }
	public bool RequiereCambioContraseña { get; set; } = false;

	public string? SemillaMfa { get; set; }
	public bool MfaHabilitado { get; set; }
	public bool MfaVerificado { get; set; } = false;
}

public class Sesion
{
	[Key] public string SessionId { get; set; }
	public Usuario Usuario { get; set; }
	public DateTime FechaCreacion { get; set; }
	public DateTime FechaExpiracion { get; set; }
	public bool Invalidada { get; set; }

	public bool EsInvalida => FechaExpiracion < DateTime.Now || Invalidada;

	public Sesion()
	{
		byte[] b = new byte[32];
		Random.Shared.NextBytes(b);

		SessionId = Convert.ToHexString(b);
		FechaCreacion = DateTime.Now;
	}

	public Sesion(Usuario u, DateTime fechaExpiracion) : this()
	{
		FechaExpiracion = fechaExpiracion;
		Usuario = u;
	}
}