using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Stripe;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class SubscribeModel : PageModel
{
	private readonly ApplicationDatabaseContext _context;
	private readonly IConfiguration _configuration;

	public SubscribeModel(ApplicationDatabaseContext context, IConfiguration configuration)
	{
		_configuration = configuration;
		_context = context;
	}
	
	public string StripePublishable { get; set; } = null!;
	
	public IActionResult OnGet()
	{
		CustomerService customerService = new();
		// TODO: Crear customer si no existe
		
		StripePublishable = _configuration["Stripe:Publishable"];
		return Page();
	}
}