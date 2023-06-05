using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

[Authorize(Roles = Entrenador.RoleName)]
public class DashboardModel : EntrenadorPageModel
{
	public DashboardModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(userManager, databaseContext)
	{
	}

	public int Alumnos;

	public IActionResult OnGet()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("Subscribe");
		
		Alumnos = DatabaseContext.Alumnos.Count(a => a.Entrenador == Entrenador);
		
		return Page();
	}
}