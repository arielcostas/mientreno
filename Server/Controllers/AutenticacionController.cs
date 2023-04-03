using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Compartido.Errores;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;
using Mientreno.Server.Services;

namespace Mientreno.Server.Controllers;

[Controller]
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

    [HttpPost]
    public async Task<ActionResult<LoginOutput>> Iniciar([FromBody] LoginInput loginInput)
    {
        _logger.LogInformation("Iniciando sesión con {Identificador}", loginInput.Identificador);

        try
        {
            return await _autenticacionService.Login(loginInput) ?? throw new InvalidCredentialException();
        }
        catch (InvalidCredentialsException)
        {
            _logger.LogWarning("Credenciales inválidas para {Identificador}", loginInput.Identificador);
            return Unauthorized(MensajesError.CredencialesInvalidas);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogError(e, "Error al iniciar sesión con {Identificador}", loginInput.Identificador);
            return StatusCode(500);
        }
    }

    [HttpPost]
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
            return Conflict(e.ParamName);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Refrescar([FromBody] RefrescarInput refreshInput)
    {
        _logger.LogInformation("Refrescando token");
        try
        {
            return Ok(await _autenticacionService.Refrescar(refreshInput));
        }
        catch (Exception)
        {
            return Unauthorized(MensajesError.CredencialesInvalidas);
        }
    }

    [HttpGet]
    [Authorize]
    public ActionResult Info()
    {
        var claims = User.Claims.ToList();
        string body = string.Empty;

        for (int i = 0; i < claims.Count; i++)
        {
            System.Security.Claims.Claim? claim = claims[i];
            body += $"{claim.Type} => {claim.Value}\r\n";
        }
        return Ok(body);
    }
}