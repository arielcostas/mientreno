using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Alumnos;

[Authorize(Roles = Entrenador.RoleName)]
public class AlumnosModel : EntrenadorPageModel
{
	public AlumnosModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}

	public Alumno[] Alumnos { get; set; } = Array.Empty<Alumno>();
	public string? EnlaceInvitacion { get; set; }

	public IActionResult OnGet()
	{
		LoadEntrenador();

		Alumnos = DatabaseContext.Alumnos
			.Where(a => a.Entrenador == Entrenador)
			.ToArray();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadEntrenador();

		if (!ModelState.IsValid) return Page();

		Invitacion invitacion = new()
		{
			Entrenador = Entrenador,
			Usos = 0,
			MaximoUsos = byte.Parse(Request.Form["cantidad"].FirstOrDefault() ?? "0")
		};

		Entrenador.Invitaciones.Add(invitacion);
		DatabaseContext.Update(Entrenador);
		await DatabaseContext.SaveChangesAsync();

		EnlaceInvitacion = Url.Page(
			"/Register",
			null,
			new { area = "", inv = invitacion.Id },
			Request.Scheme
		);

		return Page();
	}
}