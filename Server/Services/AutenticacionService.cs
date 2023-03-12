using Microsoft.AspNetCore.Identity;
using Server.Models;
using Server.RestParams;

namespace Server.Services;

public class AutenticacionService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly TokenGenerator _tokenGenerator;
    private readonly ILogger<AutenticacionService> _logger;

    public AutenticacionService(AppDbContext context, TokenGenerator tokenGenerator,
        ILogger<AutenticacionService> logger)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Usuario>();
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<LoginOutput?> Login(LoginInput loginInput)
    {
        return await LoginConContraseña(loginInput);
    }

    private async Task<LoginOutput?> LoginConContraseña(LoginInput loginInput)
    {
        var usuarioEncontrado = _context.Usuarios
            .FirstOrDefault(
                u => u.Login == loginInput.Identificador || u.Credenciales.Email == loginInput.Identificador);

        if (usuarioEncontrado == null)
        {
            // TODO: Hay que comprobar que sea 2FA
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuarioEncontrado,
            usuarioEncontrado.Credenciales.Contraseña, loginInput.Credencial);

        switch (resultado)
        {
            case PasswordVerificationResult.Failed:
                return null;
            case PasswordVerificationResult.SuccessRehashNeeded:
                usuarioEncontrado.Credenciales.Contraseña = _passwordHasher.HashPassword(usuarioEncontrado,
                    loginInput.Credencial);
                await _context.SaveChangesAsync();
                break;
            case PasswordVerificationResult.Success:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // TODO: Comprobar si hace falta 2FA
        string rol = usuarioEncontrado switch
        {
            Entrenador => "Entrenador",
            Alumno => "Alumno",
            _ => throw new Exception("Usuario no es ni Entrenador ni Alumno")
        };

        var tokenAcceso = _tokenGenerator.GenerarToken(
            TipoToken.Acceso,
            DateTime.Now.AddMinutes(120),
            usuarioEncontrado.Id.ToString(),
            usuarioEncontrado.Login,
            rol
        );

        var tokenRefresco = _tokenGenerator.GenerarToken(
            TipoToken.Refresco,
            DateTime.Now.AddDays(14),
            usuarioEncontrado.Id.ToString(),
            usuarioEncontrado.Login,
            rol
        );

        return new LoginOutput
        {
            RequiereDesafio = false,
            TokenAcceso = tokenAcceso,
            TokenRefresco = tokenRefresco,
        };
    }

    private LoginOutput LoginConTotp(LoginInput loginInput)
    {
        throw new NotImplementedException();
    }
}