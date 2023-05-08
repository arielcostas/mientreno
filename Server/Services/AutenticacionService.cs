using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Compartido.Errores;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Crypto;
using Mientreno.Server.Helpers.Queue;
using Mientreno.Server.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Sentry;
using BC = BCrypt.Net.BCrypt;

namespace Mientreno.Server.Services;

public class AutenticacionService
{
	private const string TotpPrefix = "__";

	private readonly AppDbContext _context;
	private readonly ILogger<AutenticacionService> _logger;
	private readonly TokenGenerator _tokenGenerator;
	private readonly IQueueProvider _queueProvider;

	public AutenticacionService(AppDbContext context, TokenGenerator tokenGenerator,
		ILogger<AutenticacionService> logger, IQueueProvider queueProvider)
	{
		_context = context;
		_tokenGenerator = tokenGenerator;
		_logger = logger;
		_queueProvider = queueProvider;
	}

	public async Task<LoginOutput?> Login(LoginInput loginInput)
	{
		/*if (loginInput.Identificador.StartsWith(TOTP_PREFIX))
		{
			return await LoginConTotp(loginInput);
		}*/

		return await LoginConContraseña(loginInput);
	}

	public async Task<RefrescarOutput> Refrescar(RefrescarInput refreshInput)
	{
		var tokenActual = _tokenGenerator.VerificarTokenMac(refreshInput.TokenRefresco);

		var isRefresh = tokenActual[ExtraClaims.TokenType] == TokenTypes.Refresh;
		if (!isRefresh)
		{
			throw new InvalidCredentialsException();
		}

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
			{ ClaimTypes.Name, usuarioSesion.Email },
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
				{ ExtraClaims.Nonce, storedSession.SessionId },
				{ ExtraClaims.TokenType, TokenTypes.Refresh }
			}
		);

		return new RefrescarOutput
		{
			TokenAcceso = tokenAcceso,
			TokenRefresco = tokenRefresco
		};
	}

	public async Task Registrar(RegistroInput registroInput)
	{
		var conflict = await _context.Usuarios
			.FirstOrDefaultAsync(u => u.Email == registroInput.Correo);

		if (conflict != null)
		{
			throw new ArgumentException("Correo ya en uso", nameof(conflict.Email));
		}

		var c1 = SentrySdk.GetSpan()!.StartChild("bc.hash", "Hash password");
		var hash = BC.HashPassword(registroInput.Contraseña);
		c1.Finish();

		Usuario user = new()
		{
			Id = Guid.NewGuid(),
			Email = registroInput.Correo,
			Nombre = registroInput.Nombre,
			Apellidos = registroInput.Apellido,
			FechaCreacion = DateTime.Now,
			CodigoVerificacionEmail = GenerarCodigoVerificacionEmail(),
			EmailVerificado = false,
			Credenciales = new Credenciales
			{
				Contraseña = hash,
				MfaHabilitado = false,
			}
		};

		_context.Entrenadores.Add(new Entrenador(user));

		await _context.SaveChangesAsync();

		_queueProvider.Enqueue(Constantes.ColaEmails, new Compartido.Mensajes.Email
		{
			DireccionDestinatario = user.Email,
			Idioma = "es",
			NombreDestinatario = $"{user.Apellidos}, {user.Nombre}",
			Parametros = new[]
			{
				user.Nombre, user.CodigoVerificacionEmail!,
				UrlEncoder.Default.Encode(user.Email)
			},
			Plantila = Constantes.EmailConfirmar
		});
	}

	public async Task Confirmar(ConfirmarInput input)
	{
		_logger.LogInformation("Confirmando cuenta de {DireccionEmail}", input.DireccionEmail);

		var user = _context.Usuarios
			           .Where(u => u.Email == input.DireccionEmail)
			           .Where(u => u.EmailVerificado == false)
			           .FirstOrDefault(u => u.CodigoVerificacionEmail == input.CodigoVerificacion)
		           ?? throw new UserNotFoundException();

		user.EmailVerificado = true;
		user.CodigoVerificacionEmail = null;

		await _context.SaveChangesAsync();

		_logger.LogInformation("Cuenta de {DireccionEmail} confirmada", input.DireccionEmail);

		_queueProvider.Enqueue(Constantes.ColaEmails, new Compartido.Mensajes.Email
		{
			DireccionDestinatario = user.Email,
			Idioma = "es",
			NombreDestinatario = $"{user.Apellidos}, {user.Nombre}",
			Parametros = new[] { user.Nombre },
			Plantila = Constantes.EmailBienvenida
		});

		_logger.LogInformation("Correo de bienvenida programado para enviar a {Email}", user.Email);
	}

	private static string GenerarCodigoVerificacionEmail()
	{
		var buffer = new byte[32];
		Random.Shared.NextBytes(buffer);

		return Convert.ToHexString(buffer);
	}

	private async Task<LoginOutput?> LoginConContraseña(LoginInput loginInput)
	{
		SentrySdk.ConfigureScope(o => { o.TransactionName = "LoginConContraseña"; });

		var usuarioEncontrado = await _context.Usuarios
			                        .Include(u => u.Sesiones)
			                        .FirstOrDefaultAsync(u => u.Email == loginInput.Identificador) ??
		                        throw new InvalidCredentialsException();

		if (!usuarioEncontrado.EmailVerificado)
		{
			throw new EmailNoConfirmadoException();
		}

		bool resultado = false;
		try
		{
			resultado = BC.Verify(usuarioEncontrado.Credenciales.Contraseña, loginInput.Credencial);
		}
		catch (Exception e)
		{
			SentrySdk.CaptureException(e);
			_logger.LogError("Error verifying password hash: " + e.Message);
		}

		if (!resultado)
		{
			throw new InvalidCredentialException();
		}

		if (usuarioEncontrado.Credenciales.MfaHabilitado)
		{
			var codigo = _tokenGenerator.GenerarTokenMac(
				DateTime.Now.AddMinutes(10),
				new Dictionary<string, string>
				{
					{ "id", usuarioEncontrado.Id.ToString() },
					{ ExtraClaims.TokenType, TokenTypes.Challange }
				}
			);

			return new LoginOutput
			{
				RequiereDesafio = true,
				Codigo = TotpPrefix + codigo
			};
		}

		var rol = usuarioEncontrado switch
		{
			Entrenador => "Entrenador",
			Alumno => "Alumno",
			_ => throw new ArgumentOutOfRangeException(
				"userType",
				usuarioEncontrado.GetType(),
				$@"Usuario con {usuarioEncontrado.Id} no es  alumno ni entrenador"
			)
		};

		var now = DateTime.Now;
		var sess = new Sesion(usuarioEncontrado, now.AddDays(14));

		Dictionary<string, string> datos = new()
		{
			{ ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString() },
			{ ClaimTypes.Name, usuarioEncontrado.Email },
			{ ClaimTypes.GivenName, usuarioEncontrado.Nombre },
			{ ClaimTypes.Surname, usuarioEncontrado.Apellidos },
			{ ClaimTypes.Role, rol },
			{ "nonce", sess.SessionId },
			{ ExtraClaims.TokenType, TokenTypes.Refresh }
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
}