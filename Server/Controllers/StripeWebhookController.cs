using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Stripe;

namespace Mientreno.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeWebhookController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly ApplicationDatabaseContext _context;

	public StripeWebhookController(IConfiguration configuration, ApplicationDatabaseContext context)
	{
		_configuration = configuration;
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> Get()
	{
		var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
		
		var stripeEvent = EventUtility.ConstructEvent(
			json,
			Request.Headers["Stripe-Signature"],
			_configuration["Stripe:Webhook"]
		);
		
		switch (stripeEvent.Type)
		{
			case Events.CustomerSubscriptionCreated:
				return await HandleCustomerSubscriptionCreated(stripeEvent);
			case Events.CustomerSubscriptionUpdated:
				return await HandleCustomerSubscriptionUpdated(stripeEvent);
			case Events.CustomerSubscriptionDeleted:
				return await HandleCustomerSubscriptionDeleted(stripeEvent);
		}

		return Ok();
	}

	private async Task<IActionResult> HandleCustomerSubscriptionCreated(Event stripeEvent)
	{
		var subscription = stripeEvent.Data.Object as Subscription;
		
		if (subscription is null) return BadRequest();

		var entrenador = _context.Entrenadores
			.FirstOrDefault(e => e.Suscripcion.CustomerId == subscription.CustomerId);

		if (entrenador is null) return NotFound();

		var product = await new ProductService().GetAsync(subscription.Items.Data[0].Plan.ProductId);
		entrenador.Suscripcion.Plan = product.Metadata["mientreno_code"]!;
		entrenador.Suscripcion.Estado = EstadoSuscripcion.Activa;
		entrenador.Suscripcion.FechaInicio = subscription.CurrentPeriodStart;
		entrenador.Suscripcion.FechaFin = subscription.CurrentPeriodEnd;
		entrenador.Suscripcion.RenovacionAutomatica = !subscription.CancelAtPeriodEnd;

		await _context.SaveChangesAsync();

		return Ok();
	}

	private async Task<IActionResult> HandleCustomerSubscriptionUpdated(Event stripeEvent)
	{
		throw new NotImplementedException();
	}

	private async Task<IActionResult> HandleCustomerSubscriptionDeleted(Event stripeEvent)
	{
		throw new NotImplementedException();
	}
}