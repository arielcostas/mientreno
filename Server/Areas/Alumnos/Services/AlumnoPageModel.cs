using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Alumnos.Services;

[Authorize(Roles = Alumno.RoleName)]
public class AlumnoPageModel : PageModel
{
	protected readonly UserManager<Usuario> UserManager;
	protected readonly ApplicationDatabaseContext DatabaseContext;

	public AlumnoPageModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext)
	{
		UserManager = userManager;
		DatabaseContext = databaseContext;
		Alumno = null!;
	}

	private bool _loaded;
	public Alumno Alumno;

	protected void LoadAlumno(
		bool? includeEntrenador = true,
		bool? includeJornadas = false,
		bool? includeCuestionarios = false
	)
	{
		if (_loaded) return;

		var user = UserManager.GetUserAsync(User).Result!;

		var ent = DatabaseContext.Alumnos as IQueryable<Alumno>;

		if (includeEntrenador == true)
		{
			ent = ent.Include(e => e.Entrenador);
		}

		if (includeJornadas == true)
		{
			ent = ent.Include(e => e.JornadasEntrenamientos)
				.ThenInclude(j => j.Ejercicios)
				.ThenInclude(ep => ep.Ejercicio);
		}

		if (includeCuestionarios == true)
		{
			ent = ent.Include(e => e.Cuestionarios);
		}
		
		Alumno = ent.AsSplitQuery().FirstOrDefault(e => e.Id == user.Id)!;

		_loaded = true;
	}
}