using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Compartido.Errores;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Services;

namespace Mientreno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AutenticacionController : ControllerBase
{
	private readonly AutenticacionService _autenticacionService;
	private readonly ILogger<AutenticacionController> _logger;

	public AutenticacionController(AutenticacionService autenticacionService, ILogger<AutenticacionController> logger)
	{
		_autenticacionService = autenticacionService;
		_logger = logger;
	}

	/// <summary>
	/// Inicia sesión
	/// </summary>
	/// <param name="loginInput">El identificador (nombre de usuario, email o código de desafío) y la credencial (contraseña o código TOTP).</param>
	/// <returns>Información sobre si el login fue exitoso, o si se require completar un desafío 2FA con TOTP.</returns>
	/// <response code="200">Inicio de sesión correcto.</response>
	/// <response code="401">Si las credenciales no coinciden.</response>
	[HttpPost]
	[ProducesResponseType(typeof(LoginOutput), 200)]
	[ProducesResponseType(401)]
	public async Task<ActionResult<LoginOutput>> Iniciar([FromBody] LoginInput loginInput)
	{
		_logger.LogInformation("Iniciando sesión con {Identificador}", loginInput.Identificador);

		try
		{
			return await _autenticacionService.Login(loginInput) ?? throw new InvalidCredentialException();
		}
		catch (EmailNoConfirmadoException e)
		{
			_logger.LogWarning("Email no confirmado para {Identificador}", loginInput.Identificador);
			throw new HttpRequestException(MensajesError.EmailNoConfirmado, e, HttpStatusCode.Unauthorized);
		}
		catch (InvalidCredentialsException e)
		{
			_logger.LogWarning("Credenciales inválidas para {Identificador}", loginInput.Identificador);
			throw new HttpRequestException(MensajesError.CredencialesInvalidas, e, HttpStatusCode.Unauthorized);
		}
		catch (ArgumentOutOfRangeException e)
		{
			_logger.LogError(e, "Error al iniciar sesión con {Identificador}", loginInput.Identificador);
			return StatusCode(500);
		}
	}

	/// <summary>
	/// Dar de alta nuevo usuario.
	/// </summary>
	/// <param name="registroInput">Los datos básicos del nuevo usuario</param>
	/// <response code="204">El usuario se registró correctamente</response>
	/// <response code="409">Si hay algún dato duplicado. El `details` puede ser `LOGIN_EXISTS` o `EMAIL_EXISTS`</response>
	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(409)]
	public async Task<ActionResult> Registrar([FromBody] RegistroInput registroInput)
	{
		_logger.LogInformation("Registrando usuario con login {Login}", registroInput.Login);
		try
		{
			await _autenticacionService.Registrar(registroInput);
			return NoContent();
		}
		catch (ArgumentException e)
		{
			_logger.LogWarning($"Se intentó hacer un registro con un {e.ParamName} existente");
			if (e.ParamName == "login")
				throw new HttpRequestException(MensajesError.LoginExistente, e, HttpStatusCode.Conflict);
			else if (e.ParamName == "email")
				throw new HttpRequestException(MensajesError.EmailExistente, e, HttpStatusCode.Conflict);
			else
				return StatusCode(500);
		}
	}

	/// <summary>
	/// Refrescar sesión (obtener nuevo par de tokens)
	/// </summary>
	/// <param name="refreshInput">El token de refresco</param>
	/// <returns>Un nuevo token de acceso, y un nuevo token de refresco extendido, para remplazar el actual.</returns>
	/// <response code="200">Se ha emitido un token de acceso y de refresco nuevos.</response>
	/// <response code="401">Si se ha enviado un token de refresco inválido.</response>
	/// <response code="403">Si el token es válido, pero por algún motivo no se puede refrescar. Se debe hacer un nuevo inicio de sesión.</response>
	[HttpPost]
	[ProducesResponseType(typeof(RefrescarOutput), 200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	public async Task<ActionResult<RefrescarOutput>> Refrescar([FromBody] RefrescarInput refreshInput)
	{
		_logger.LogInformation("Refrescando token");
		try
		{
			return await _autenticacionService.Refrescar(refreshInput);
		}
		catch (InvalidCredentialsException e)
		{
			_logger.LogWarning("Token no usable para refrescar.");
			throw new HttpRequestException(MensajesError.CredencialesInvalidas, e, HttpStatusCode.Unauthorized);
		}
		catch (SecurityTokenExpiredException e)
		{
			_logger.LogWarning("Token o sesión expirados.");
			throw new HttpRequestException(MensajesError.CredencialesInvalidas, e, HttpStatusCode.Forbidden);
		}
	}

	/// <summary>
	/// Confirmar correo electrónico.
	/// </summary>
	/// <param name="input">Valores necesarios para confirmar (código único y dirección de correo).</param>
	/// <response code="204">Si se ha confirmado correctamente el correo.</response>
	/// <response code="401">Si no se ha confirmado, o porque ya se hizo anteriormente o porque los parámetros son inválidos.</response>
	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(401)]
	public async Task<ActionResult> Confirmar([FromBody] ConfirmarInput input)
	{
		_logger.LogInformation($"Confirmando usuario con email {input.DireccionEmail}");
		try
		{
			await _autenticacionService.Confirmar(input);
			return NoContent();
		}
		catch (UserNotFoundException)
		{
			_logger.LogWarning($"Usuario pendiente de verificar no encontrado con email {input.DireccionEmail}");
			return BadRequest();
		}
	}
}