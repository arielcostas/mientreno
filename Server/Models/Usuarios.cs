using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Mientreno.Server.Models;

/// <summary>
/// Un usuario genérico, con su ID, datos básicos y credenciales.
/// </summary>
public class Usuario : IdentityUser
{
	public string Nombre { get; set; } = string.Empty;
	public string Apellidos { get; set; } = string.Empty;

	public DateTime FechaAlta { get; set; }
	public DateTime? FechaEliminacion { get; set; }
}

/// <summary>
/// Un entrenador es un usuario que tiene alumnos a los que les asigna rutinas, objetivos, etc.
/// </summary>
public class Entrenador : Usuario
{
	public const string RoleName = "Entrenador";
	public List<Alumno> Alumnos { get; set; } = new();

	public List<Ejercicio> Ejercicios { get; set; } = new();
	public List<Categoria> Categorias { get; set; } = new();

	public Entrenador()
	{
	}
}

/// <summary>
/// Un alumno es un usuario que recibe entrenamiento de un entrenador.
/// </summary>
public class Alumno : Usuario
{
	public const string RoleName = "Alumno";
	public Entrenador Entrenador { get; set; }
	public List<Cuestionario> Cuestionarios { get; set; } = new();

	public Alumno()
	{
	}
}