using Microsoft.AspNetCore.Mvc;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service.Queue;
using Stripe;

namespace Mientreno.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeWebhookController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly ApplicationDatabaseContext _context;
	private readonly IQueueProvider _queueProvider;

	public StripeWebhookController(IConfiguration configuration, ApplicationDatabaseContext context,
		IQueueProvider queueProvider)
	{
		_configuration = configuration;
		_context = context;
		_queueProvider = queueProvider;
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
			case Events.CustomerSubscriptionTrialWillEnd:
				return HandleTrialWillEnd(stripeEvent);
		}

		// TODO: Gestionar facturas pagadas

		return Ok();
	}

	private IActionResult HandleTrialWillEnd(Event stripeEvent)
	{
		if (stripeEvent.Data.Object is not Subscription subscription) return BadRequest();
		
		var entrenador = _context.Entrenadores
			.FirstOrDefault(e => e.Suscripcion.CustomerId == subscription.CustomerId);
		
		if (entrenador is null) return NotFound();

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = Idiomas.Castellano,
			NombreDestinatario = entrenador.NombreCompleto,
			DireccionDestinatario = entrenador.Email!,
			Plantila = Constantes.PruebaTermina,
			Parametros = new[] { entrenador.Nombre, entrenador.Suscripcion.Plan.ToString()! }
		});

		return Ok();
	}
	
	private async Task<IActionResult> HandleSubscriptionStatusChange(Event stripeEvent)
	{
		if (stripeEvent.Data.Object is not Subscription subscription) return BadRequest();

		var customer = await new CustomerService().GetAsync(subscription.CustomerId);

		var entrenador = _context.Entrenadores
			.FirstOrDefault(e => e.Email == customer.Email);

		if (entrenador is null) return NotFound();

		var product = await new ProductService().GetAsync(subscription.Items.Data[0].Plan.ProductId);

		var plan = ParsePlan(product.Metadata["mientreno_code"]!);
		if (plan is null) return BadRequest();

		entrenador.Suscripcion.Plan = plan;
		entrenador.Suscripcion.Estado = subscription.Status switch
		{
			SubscriptionStatuses.Trialing => EstadoSuscripcion.Activa,
			SubscriptionStatuses.Active => EstadoSuscripcion.Activa,
			SubscriptionStatuses.Canceled => EstadoSuscripcion.Cancelada,
			SubscriptionStatuses.PastDue => EstadoSuscripcion.Expirada,
			SubscriptionStatuses.Unpaid => EstadoSuscripcion.Expirada,
			SubscriptionStatuses.Incomplete => EstadoSuscripcion.NoSuscrito,
			SubscriptionStatuses.IncompleteExpired => EstadoSuscripcion.NoSuscrito,
			_ => EstadoSuscripcion.NoSuscrito
		};
		entrenador.Suscripcion.FechaInicio = subscription.CurrentPeriodStart;
		entrenador.Suscripcion.FechaFin = subscription.CurrentPeriodEnd;
		entrenador.Suscripcion.RenovacionAutomatica = !subscription.CancelAtPeriodEnd;

		await _context.SaveChangesAsync();

		switch (stripeEvent.Type)
		{
			case Events.CustomerSubscriptionCreated:
				entrenador.Suscripcion.CustomerId = customer.Id;

				_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
				{
					Idioma = Idiomas.Castellano,
					NombreDestinatario = entrenador.NombreCompleto,
					DireccionDestinatario = entrenador.Email!,
					Plantila = Constantes.SuscripcionCreada,
					// TODO: Los planes deberían usar el nombre bien capitalizado
					Parametros = new[] { entrenador.Nombre, plan.ToString()! }
				});
				break;
			case Events.CustomerSubscriptionDeleted:
				_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
				{
					Idioma = Idiomas.Castellano,
					NombreDestinatario = entrenador.NombreCompleto,
					DireccionDestinatario = entrenador.Email!,
					Plantila = Constantes.SuscripcionCancelada,
					Parametros = new[] { entrenador.NombreCompleto }
				});
				break;
		}

		return Ok();
	}

	private static PlanSuscripcion? ParsePlan(string nombre)
	{
		return nombre switch
		{
			"estandar" => PlanSuscripcion.Estandar,
			"prime" => PlanSuscripcion.Prime,
			"basico" => PlanSuscripcion.Basic,
			_ => null
		};
	}
}