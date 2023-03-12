using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.RestParams;
using Server.Services;

namespace Server;

[Controller]
[Route("api/[controller]/[action]")]
public class AutenticacionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly TokenGenerator _tokenGenerator;
    private readonly AutenticacionService _autenticacionService;
    private readonly ILogger<AutenticacionController> _logger;

    public AutenticacionController(AppDbContext context, TokenGenerator tokenGenerator,
        AutenticacionService autenticacionService, ILogger<AutenticacionController> logger)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _autenticacionService = autenticacionService;
        _logger = logger;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    [HttpPost("Iniciar")]
    public async Task<ActionResult<LoginOutput>> Login([FromBody] LoginInput loginInput)
    {
        _logger.LogInformation("Iniciando sesión con {Identificador}", loginInput.Identificador);
        
        var loginOutput = await _autenticacionService.Login(loginInput);
        
        if (loginOutput == null)
        {
            _logger.LogInformation("No se pudo iniciar sesión con {Identificador}", loginInput.Identificador);
            return BadRequest("No se pudo iniciar sesión");
        }
        
        _logger.LogInformation("Sesión iniciada con {Identificador}", loginInput.Identificador);
        return loginOutput;
    }
}