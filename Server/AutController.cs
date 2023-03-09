using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Server.RestParams;

namespace Server;

[Controller]
public class AutController : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginInput loginInput)
    {
        if (loginInput.TwoFactorCode != null)
        {
            Console.WriteLine("Doing two factor verification");
            var username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return Unauthorized();
            }

            return PerformTwoFactorVerification(username, loginInput.TwoFactorCode);
        }

        if (loginInput is { Username: { }, Password: { } })
        {
            Console.WriteLine("Doing cookie login");
            return await PerformCookieLogin(loginInput.Username, loginInput.Password);
        }

        Console.WriteLine("Bad request");
        return BadRequest();
    }

    [HttpGet("Login")]
    public IActionResult GetMyClaims()
    {
        return Ok(HttpContext.User.Claims.Select(x => new { x.Type, x.Value }));
    }

    private IActionResult PerformTwoFactorVerification(string username, string totp)
    {
        if (totp == "123456")
        {
            // Updates the `TwoFactorVerified` claim in the HttpContext.User
            
            
            
            HttpContext.SignInAsync(HttpContext.User);

            return NoContent();
        }

        return Unauthorized();
    }

    private async Task<IActionResult> PerformCookieLogin(string username, string password)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Role, password),
            new Claim("TwoFactorRequired", "true"),
            new Claim("TwoFactorVerified", "false")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimsPrincipal);

        return NoContent();
    }
}