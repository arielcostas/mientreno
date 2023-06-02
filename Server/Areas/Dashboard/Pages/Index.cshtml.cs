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

		Entrenador = null!;
		Alumnos = 0;
	}

	public Entrenador Entrenador;
	public int Alumnos;

	public async Task<IActionResult> OnGet()
	{
		Entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;
		
		Alumnos = _databaseContext.Alumnos.Count(a => a.Entrenador == Entrenador);
		
		return Page();
	}
}