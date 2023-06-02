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
			case Events.CustomerSubscriptionUpdated:
			case Events.CustomerSubscriptionDeleted:
			case Events.CustomerSubscriptionResumed:
			case Events.CustomerSubscriptionPaused:
				return await HandleSubscriptionStatusChange(stripeEvent);
		}

		// TODO: Gestionar facturas pagadas
		
		return Ok();
	}

	private async Task<IActionResult> HandleSubscriptionStatusChange(Event stripeEvent)
	{
		if (stripeEvent.Data.Object is not Subscription subscription) return BadRequest();

		var entrenador = _context.Entrenadores
			.FirstOrDefault(e => e.Suscripcion.CustomerId == subscription.CustomerId);

		if (entrenador is null) return NotFound();

		var product = await new ProductService().GetAsync(subscription.Items.Data[0].Plan.ProductId);

		var planValido = Enum.TryParse<PlanSuscripcion>(product.Metadata["mientreno_code"]!, out var plan);

		if (!planValido) return BadRequest();

		entrenador.Suscripcion.Plan = plan;
		entrenador.Suscripcion.Estado = subscription.Status switch
		{
			SubscriptionStatuses.Trialing => EstadoSuscripcion.Activa,
			SubscriptionStatuses.Active => EstadoSuscripcion.Activa,
			SubscriptionStatuses.PastDue => EstadoSuscripcion.Expirada,
			SubscriptionStatuses.Canceled => EstadoSuscripcion.Expirada,
			SubscriptionStatuses.Unpaid => EstadoSuscripcion.Expirada,
			SubscriptionStatuses.Incomplete => EstadoSuscripcion.NoSuscrito,
			SubscriptionStatuses.IncompleteExpired => EstadoSuscripcion.NoSuscrito,
			_ => EstadoSuscripcion.NoSuscrito
		};
		entrenador.Suscripcion.FechaInicio = subscription.CurrentPeriodStart;
		entrenador.Suscripcion.FechaFin = subscription.CurrentPeriodEnd;
		entrenador.Suscripcion.RenovacionAutomatica = !subscription.CancelAtPeriodEnd;

		await _context.SaveChangesAsync();

		if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
		{
			//TODO: Email confirmación suscripción	
		}

		return Ok();
	}
}