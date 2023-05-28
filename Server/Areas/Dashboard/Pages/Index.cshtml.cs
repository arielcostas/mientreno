using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class DashboardModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;
	private readonly ApplicationContext _context;
	
	public DashboardModel(UserManager<Usuario> userManager, ApplicationContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public Subscription UserSubscription;
	public Entrenador Entrenador;
	public int Alumnos;

	public async Task<IActionResult> OnGet()
	{
		UserSubscription = new Subscription("Basic", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(20), true);
		Entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;
		
		Alumnos = _context.Alumnos.Count(a => a.Entrenador == Entrenador);
		
		return Page();
	}
}

public record Subscription(
	string PlanName,
	DateTime StartDate,
	DateTime EndDate,
	bool IsAutoRenewal
)
{
	public int PercentageCompleted => (int) Math.Round((DateTime.Now - StartDate).TotalDays / (EndDate - StartDate).TotalDays * 100);
}