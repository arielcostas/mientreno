using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Helpers;
using Server.Models;
using Server.RestParams;

namespace Server.Services;

public class AutenticacionService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly TokenGenerator _tokenGenerator;
    private readonly ILogger<AutenticacionService> _logger;

    public AutenticacionService(AppDbContext context, TokenGenerator tokenGenerator,
        ILogger<AutenticacionService> logger)
    {
        _context = context;
        _passwordHasher = new Argon2PasswordHasher<Usuario>();
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<LoginOutput?> Login(LoginInput loginInput)
    {
        if (loginInput.Identificador.StartsWith("__"))
        {
            return await LoginConTotp(loginInput);
        }

        return await LoginConContraseña(loginInput);
    }

    private async Task<LoginOutput?> LoginConContraseña(LoginInput loginInput)
    {
        var usuarioEncontrado = _context.Usuarios
            .FirstOrDefault(
                u => u.Login == loginInput.Identificador || u.Credenciales.Email == loginInput.Identificador);

        if (usuarioEncontrado == null)
        {
            throw new InvalidCredentialsException();
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuarioEncontrado,
            usuarioEncontrado.Credenciales.Contraseña, loginInput.Credencial);

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (resultado)
        {
            case PasswordVerificationResult.Failed:
                throw new InvalidCredentialsException();
            case PasswordVerificationResult.SuccessRehashNeeded:
                usuarioEncontrado.Credenciales.Contraseña = _passwordHasher.HashPassword(usuarioEncontrado,
                    loginInput.Credencial);
                await _context.SaveChangesAsync();
                break;
        }


        var rol = usuarioEncontrado switch
        {
            Entrenador => "Entrenador",
            Alumno => "Alumno",
            _ => throw new ArgumentOutOfRangeException(
                "userType",
                usuarioEncontrado.GetType(),
                $"Usuario con {usuarioEncontrado.Id.ToString()} no es  alumno ni entrenador"
            )
        };

        if (usuarioEncontrado.Credenciales.MfaHabilitado)
        {
            var codigo = _tokenGenerator.GenerarTokenDesafio(
                DateTime.Now.AddMinutes(10),
                new Dictionary<string, string>()
                {
                    { "id", usuarioEncontrado.Id.ToString() }
                }
            );

            return new LoginOutput()
            {
                RequiereDesafio = true,
                Codigo = "__" + codigo // TODO: Hacer el '__' una constante
            };
        }

        var tokenAcceso = _tokenGenerator.GenerarTokenAcceso(
            DateTime.Now.AddMinutes(120),
            usuarioEncontrado.Id.ToString(),
            usuarioEncontrado.Login,
            rol
        );

        var tokenRefresco = _tokenGenerator.GenerarTokenRefresco(
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

    private async Task<LoginOutput> LoginConTotp(LoginInput loginInput)
    {
        _logger.LogInformation("Iniciando sesión con TOTP");
        throw new NotImplementedException();
    }

    public async Task Registrar(RegistroInput registroInput)
    {
        // Comprueba que el login y el correo no estén en uso
        if (_context.Usuarios.Any(u => u.Login == registroInput.Login))
        {
            throw new ArgumentException("Login ya en uso", registroInput.Login);
        }

        if (_context.Usuarios.Any(u => u.Credenciales.Email == registroInput.Correo))
        {
            throw new ArgumentException("Correo ya en uso", registroInput.Correo);
        }

        var user = new Usuario()
        {
            Id = Guid.NewGuid(),
            Login = registroInput.Login,
            Nombre = registroInput.Nombre,
            Apellidos = registroInput.Apellido,
            FechaCreacion = DateTime.Now,
            Credenciales = new Credenciales
            {
                Email = registroInput.Correo,
                CodigoVerificacionEmail = GenerarCodigoVerificacionEmail(),
                Contraseña = _passwordHasher.HashPassword(null!, registroInput.Password),
                MfaHabilitado = false,
            }
        };

        _logger.LogInformation("Registrando usuario {Login}", user.Login);

        if (registroInput.EsEntrenador)
        {
            _context.Entrenadores.Add(new Entrenador(user));
        }
        else
        {
            _context.Alumnos.Add(new Alumno(user));
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Boolean> IsSessionValid(string sessid, string userId)
    {
        var userGuid = Guid.Parse(userId);
        var s = _context.Sesiones
            .Include(s => s.Usuario)
            .Where(s => s.Usuario.Id == userGuid)
            .FirstOrDefault(s => s.SessionId == sessid);

        return s != null;
    }
    
    private string GenerarCodigoVerificacionEmail()
    {
        var buffer = new byte[32];
        Random.Shared.NextBytes(buffer);

        return Convert.ToHexString(buffer);
    }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Credenciales inválidas")
    {
    }
}