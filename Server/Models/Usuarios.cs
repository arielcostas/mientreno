using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Mientreno.Server.Models;

/// <summary>
/// Un usuario genérico, con su ID, datos básicos y credenciales.
/// </summary>
public class Usuario
{
    [Key] public Guid Id { get; set; }
    [RegularExpression("[A-Za-z0-9]{3,20}")]
    public string Login { get; set; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEliminacion { get; set; }

    public Credenciales Credenciales { get; set; }

    public Usuario()
    {
        Id = Guid.NewGuid();
        FechaCreacion = DateTime.Now;
        Login = string.Empty;
        Nombre = string.Empty;
        Apellidos = string.Empty;
        Credenciales = new Credenciales();
    }
}

/// <summary>
/// Un alumno es un usuario que recibe entrenamiento de un entrenador.
/// </summary>
public class Alumno : Usuario
{
    public List<Contrato> Asignaciones { get; set; }

    public Contrato? AsignacionActual => Asignaciones
        .OrderByDescending(ae => ae.FechaAsignacion)
        .FirstOrDefault(a => a.FechaDesasignacion == null);

    public Alumno() : base()
    {
        Asignaciones = new List<Contrato>();
    }

    public Alumno(Usuario u)
    {
        Id = u.Id;
        Login = u.Login;
        Nombre = u.Nombre;
        Apellidos = u.Apellidos;
        FechaCreacion = u.FechaCreacion;
        FechaEliminacion = u.FechaEliminacion;
        Credenciales = u.Credenciales;
        Asignaciones = new List<Contrato>();
    }
}

/// <summary>
/// Un entrenador es un usuario que tiene alumnos a los que les asigna rutinas, objetivos, etc.
/// </summary>
public class Entrenador : Usuario
{
    public List<Contrato> Asignaciones { get; set; }

    public Entrenador() : base()
    {
        Asignaciones = new List<Contrato>();
    }

    public Entrenador(Usuario u)
    {
        Id = u.Id;
        Login = u.Login;
        Nombre = u.Nombre;
        Apellidos = u.Apellidos;
        FechaCreacion = u.FechaCreacion;
        FechaEliminacion = u.FechaEliminacion;
        Credenciales = u.Credenciales;
        Asignaciones = new List<Contrato>();
    }
}

/// <summary>
/// La relación entre un entrenador y un alumno en un momento dado.
/// </summary>
public class Contrato
{
    [Key] public Guid Id { get; set; }
    public Entrenador Entrenador { get; set; }
    public Alumno Alumno { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaDesasignacion { get; set; }
    public List<Cuestionario> Cuestionarios { get; set; }
}

/// <summary>
/// Credenciales para un usuario o entrenador.
/// </summary>
public class Credenciales
{
    [EmailAddress] public string Email { get; set; }
    public string? CodigoVerificacionEmail { get; set; }
    public bool EmailVerificado { get; set; } = false;

    public string Contraseña { get; set; }
    public bool RequiereCambioContraseña { get; set; } = false;

    public string? SemillaMfa { get; set; }
    public bool MfaHabilitado { get; set; } = false;
    public bool MfaVerificado { get; set; } = false;
}

public class Sesion
{
    [Key]
    public string SessionId { get; set; }
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