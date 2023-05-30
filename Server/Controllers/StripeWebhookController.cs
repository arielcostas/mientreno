using Microsoft.AspNetCore.Mvc;

namespace Mientreno.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeWebhookController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(new { Message = "Hola desde Stripe" });
	}
}