using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Alumnos.Id;

[Authorize(Roles = Entrenador.RoleName)]
public class SingleAlumnoModel : EntrenadorPageModel
{
	public SingleAlumnoModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
		Id = string.Empty;
	}

	public Alumno Alumno { get; set; } = null!;

	[FromRoute(Name = "id")]
	public string Id { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("Subscribe");

		Console.WriteLine(Id);

		var alumno = await DatabaseContext.Alumnos
			.Include(a => a.JornadasEntrenamientos)
			.FirstOrDefaultAsync(a => a.Id == Id && a.Entrenador == Entrenador);

		if (alumno == null) return RedirectToPage("Alumnos");

		Alumno = alumno;

		return Page();
	}
}