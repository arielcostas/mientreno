using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Compartido;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Stripe;
using Stripe.BillingPortal;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class SubscribeModel : EntrenadorPageModel
{
	private readonly ApplicationDatabaseContext _context;
	private readonly IConfiguration _configuration;
	private readonly UserManager<Usuario> _userManager;

	public SubscribeModel(ApplicationDatabaseContext context, IConfiguration configuration,
		UserManager<Usuario> userManager) : base(userManager, context)
	{
		_configuration = configuration;
		_userManager = userManager;
		_context = context;

		StripePublishable = _configuration["Stripe:Publishable"]!;
		StripePricingTable = _configuration["Stripe:PricingTable"]!;
		StripeSubscriptionManager = string.Empty;
	}

	public string StripePublishable { get; set; }
	public string StripeSubscriptionManager { get; set; }
	public string StripePricingTable { get; set; }

	public async Task<IActionResult> OnGet()
	{
		LoadEntrenador();
		
		if (Entrenador.Suscripcion.CustomerId.IsNullOrEmpty())
		{
			var customerCreateOptions = new CustomerCreateOptions
			{
				Email = Entrenador.Email,
				Name = Entrenador.Nombre,
				PreferredLocales = new List<string> { Idiomas.Castellano },
			};
			
			var customerService = new CustomerService();
			
			var customer = await customerService.CreateAsync(customerCreateOptions);
			
			Entrenador.Suscripcion = new Suscripcion
			{
				CustomerId = customer.Id,
				Estado = EstadoSuscripcion.NoSuscrito
			};
			
			await _context.SaveChangesAsync();
		}

		var sessionCreateOptions = new SessionCreateOptions
		{
			Customer = Entrenador.Suscripcion.CustomerId,
			ReturnUrl = $"{Request.Scheme}://{Request.Host}/dashboard"
		};

		var sessionService = new SessionService();
		var session = await sessionService.CreateAsync(sessionCreateOptions);

		StripeSubscriptionManager = session.Url;

		return Page();
	}
}