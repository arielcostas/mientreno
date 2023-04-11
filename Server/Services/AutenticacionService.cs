using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Compartido.Errores;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Crypto;
using Mientreno.Server.Helpers.Mailing;
using Mientreno.Server.Helpers.Services;
using Mientreno.Server.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Mientreno.Server.Services;

public class AutenticacionService
{
    const string TOTP_PREFIX = "__";

    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly TokenGenerator _tokenGenerator;
    private readonly Cartero _mailWorker;
    private readonly ILogger<AutenticacionService> _logger;

    public AutenticacionService(AppDbContext context, TokenGenerator tokenGenerator,
        ILogger<AutenticacionService> logger, Cartero mailWorkerService)
    {
        _context = context;
        _passwordHasher = new Argon2PasswordHasher<Usuario>();
        _tokenGenerator = tokenGenerator;
        _logger = logger;
        _mailWorker = mailWorkerService;
    }

    public async Task<LoginOutput?> Login(LoginInput loginInput)
    {
        if (loginInput.Identificador.StartsWith(TOTP_PREFIX))
        {
            return await LoginConTotp(loginInput);
        }

        return await LoginConContraseña(loginInput);
    }

    private async Task<LoginOutput?> LoginConContraseña(LoginInput loginInput)
    {
        var usuarioEncontrado = _context.Usuarios
            .Include(u => u.Sesiones)
            .FirstOrDefault(
                u => u.Login == loginInput.Identificador || u.Credenciales.Email == loginInput.Identificador) ?? throw new InvalidCredentialsException();

        if (!usuarioEncontrado.Credenciales.EmailVerificado)
        {
            throw new EmailNoConfirmadoException();
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
                $"Usuario con {usuarioEncontrado.Id} no es  alumno ni entrenador"
            )
        };

        if (usuarioEncontrado.Credenciales.MfaHabilitado)
        {
            var codigo = _tokenGenerator.GenerarTokenMac(
                DateTime.Now.AddMinutes(10),
                new Dictionary<string, string>()
                {
                    { "id", usuarioEncontrado.Id.ToString() }
                }
            );

            return new LoginOutput()
            {
                RequiereDesafio = true,
                Codigo = TOTP_PREFIX + codigo
            };
        }

        // TODO: Variar el tiempo de expiración del token de acceso
        var now = DateTime.Now;
        var sess = new Sesion(usuarioEncontrado, now.AddDays(14));

        Dictionary<string, string> datos = new()
        {
            { ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString() },
            { ClaimTypes.Name, usuarioEncontrado.Login },
            { ClaimTypes.GivenName, usuarioEncontrado.Nombre },
            { ClaimTypes.Surname, usuarioEncontrado.Apellidos },
            { ClaimTypes.Role, rol },
            { "nonce", sess.SessionId }
        };

        var tokenAcceso = _tokenGenerator.GenerarTokenJWT(now.AddMinutes(120), datos);

        usuarioEncontrado.Sesiones.Add(sess);

        await _context.SaveChangesAsync();

        var tokenRefresco = _tokenGenerator.GenerarTokenMac(
            now.AddDays(14),
            new Dictionary<string, string>()
            {
                { ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString() },
                { ExtraClaims.Nonce, sess.SessionId }
            }
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
        var tokenData = _tokenGenerator.VerificarTokenMac(loginInput.Identificador[2..]);

        foreach (var (k, v) in tokenData)
        {
            await Console.Out.WriteLineAsync($"{k} => {v}");
        }

        return new LoginOutput
        {
            RequiereDesafio = true,
            TokenAcceso = string.Empty,
            TokenRefresco = string.Empty
        };
    }

    public async Task Registrar(RegistroInput registroInput)
    {
        // Comprueba que el login y el correo no estén en uso
        var conflict = _context.Usuarios
            .Where(u => u.Login == registroInput.Login)
            .Where(u => u.Credenciales.Email == registroInput.Correo)
            .FirstOrDefault();

        if (conflict != null)
        {
            if (conflict.Login == registroInput.Login)
            {
                throw new ArgumentException("Login ya en uso", registroInput.Login);
            }

            if (conflict.Credenciales.Email == registroInput.Correo)
            {
                throw new ArgumentException("Correo ya en uso", registroInput.Correo);
            }
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
                Contraseña = _passwordHasher.HashPassword(null!, registroInput.Contraseña),
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

        _logger.LogInformation("Usuario {Login} registrado. Enviando correo de verificación", user.Login);

        Email email = EmailTemplate.ConfirmAddressEmail(ref user);

        _mailWorker.AddEmailToQueue(email);

        _logger.LogInformation("Correo de verificación programado para enviar a {Email}", user.Credenciales.Email);
    }

    public bool IsSessionValid(string sessid, string userId)
    {
        var userGuid = Guid.Parse(userId);
        var s = _context.Sesiones
            .Include(s => s.Usuario)
            .Where(s => s.Usuario.Id == userGuid)
            .FirstOrDefault(s => s.SessionId == sessid);

        return s != null;
    }

    private static string GenerarCodigoVerificacionEmail()
    {
        var buffer = new byte[32];
        Random.Shared.NextBytes(buffer);

        return Convert.ToHexString(buffer);
    }

    public async Task<RefrescarOutput> Refrescar(RefrescarInput refreshInput)
    {
        var tokenActual = _tokenGenerator.VerificarTokenMac(refreshInput.TokenRefresco);
        var sessionId = tokenActual[ExtraClaims.Nonce] ?? throw new InvalidCredentialsException();

        var storedSession = await _context.Sesiones
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (storedSession == null || storedSession.Invalidada)
        {
            throw new SecurityTokenExpiredException();
        }

        var usuarioSesion = storedSession.Usuario;

        var rol = usuarioSesion switch
        {
            Entrenador => "Entrenador",
            Alumno => "Alumno",
            _ => throw new InvalidCastException(
                $"Usuario con {usuarioSesion.Id} no es  alumno ni entrenador"
            )
        };

        Dictionary<string, string> datos = new()
        {
            { ClaimTypes.NameIdentifier, usuarioSesion.Id.ToString() },
            { ClaimTypes.Name, usuarioSesion.Login },
            { ClaimTypes.GivenName, usuarioSesion.Nombre },
            { ClaimTypes.Surname, usuarioSesion.Apellidos },
            { ClaimTypes.Role, rol },
            { "nonce", sessionId }
        };

        storedSession.FechaExpiracion = DateTime.Now.AddDays(14);
        await _context.SaveChangesAsync();

        var tokenAcceso = _tokenGenerator.GenerarTokenJWT(DateTime.Now.AddMinutes(120), datos);

        var tokenRefresco = _tokenGenerator.GenerarTokenMac(
            DateTime.Now.AddDays(14),
            new Dictionary<string, string>()
            {
                { ClaimTypes.NameIdentifier, usuarioSesion.Id.ToString() },
                { ExtraClaims.Nonce, storedSession.SessionId }
            }
        );

        return new RefrescarOutput
        {
            TokenAcceso = tokenAcceso,
            TokenRefresco = tokenRefresco
        };
    }

    internal async Task Confirmar(ConfirmarInput input)
    {
        _logger.LogInformation("Confirmando cuenta de {DireccionEmail}", input.DireccionEmail);

        var u = _context.Usuarios
            .Where(u => u.Credenciales.Email == input.DireccionEmail)
            .Where(u => u.Credenciales.EmailVerificado == false)
            .FirstOrDefault(u => u.Credenciales.CodigoVerificacionEmail == input.CodigoVerificacion)
                ?? throw new UserNotFoundException();

        u.Credenciales.EmailVerificado = true;
        u.Credenciales.CodigoVerificacionEmail = null;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Cuenta de {DireccionEmail} confirmada", input.DireccionEmail);

        Email email = EmailTemplate.AfterConfirmWelcomeEmail(ref u);

        _mailWorker.AddEmailToQueue(email);
    }
}
