using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class Usuario
{
    [Key]
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }

    public string Email { get; set; }
    public string? CodigoVerificacionEmail { get; set; }
    public bool EmailVerificado { get; set; } = false;

    public string Contraseña { get; set; }
    public bool RequiereCambioContraseña { get; set; } = false;

    public string SemillaMfa { get; set; }
    public bool MfaHabilitado { get; set; } = false;
    public bool MfaVerificado { get; set; } = false;

    public Usuario(Guid id, string login, string nombre, string apellidos, string email,
        string? codigoVerificacionEmail, bool emailVerificado, string contraseña, bool requiereCambioContraseña,
        string semillaMfa, bool mfaHabilitado, bool mfaVerificado)
    {
        Id = id;
        Login = login;
        Nombre = nombre;
        Apellidos = apellidos;
        Email = email;
        CodigoVerificacionEmail = codigoVerificacionEmail;
        EmailVerificado = emailVerificado;
        Contraseña = contraseña;
        RequiereCambioContraseña = requiereCambioContraseña;
        SemillaMfa = semillaMfa;
        MfaHabilitado = mfaHabilitado;
        MfaVerificado = mfaVerificado;
    }
}