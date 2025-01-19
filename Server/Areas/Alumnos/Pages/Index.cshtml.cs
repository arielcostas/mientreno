using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Alumnos.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Alumnos.Pages;

public class AlumnoIndexModel : AlumnoPageModel
{
	public AlumnoIndexModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}

	public IActionResult OnGet()
	{
		LoadAlumno(includeEntrenador: true, includeJornadas:true);
		return Page();
	}
}