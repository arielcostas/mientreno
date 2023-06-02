using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Stripe;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class SubscribeModel : PageModel
{
	private readonly ApplicationDatabaseContext _context;
	private readonly IConfiguration _configuration;
	private readonly UserManager<Usuario> _userManager;

	public SubscribeModel(ApplicationDatabaseContext context, IConfiguration configuration, UserManager<Usuario> userManager)
	{
		_configuration = configuration;
		_userManager = userManager;
		_context = context;

		StripePublishable = _configuration["Stripe:Publishable"]!;
		Entrenador = null!;
	}
	
	public string StripePublishable { get; set; }
	public Entrenador Entrenador { get; set; }
	
	public async Task<IActionResult> OnGet()
	{
		var usuario = (await _userManager.GetUserAsync(User))!;

		Entrenador = _context.Entrenadores
			.Include(e => e.Suscripcion)
			.FirstOrDefault(e => e.Id == usuario.Id)!;
		
		return Page();
	}
}