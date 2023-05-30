using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

[Authorize(Roles = Entrenador.RoleName)]
public class DashboardModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;
	private readonly ApplicationDatabaseContext _databaseContext;
	
	public DashboardModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext)
	{
		_userManager = userManager;
		_databaseContext = databaseContext;

		UserSubscription = null!;
		Entrenador = null!;
		Alumnos = 0;
	}

	public Subscription UserSubscription;
	public Entrenador Entrenador;
	public int Alumnos;

	public async Task<IActionResult> OnGet()
	{
		UserSubscription = new Subscription("Basic", DateTime.Now.AddDays(-28), DateTime.Now.AddDays(1), true);
		Entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;
		
		Alumnos = _databaseContext.Alumnos.Count(a => a.Entrenador == Entrenador);
		
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