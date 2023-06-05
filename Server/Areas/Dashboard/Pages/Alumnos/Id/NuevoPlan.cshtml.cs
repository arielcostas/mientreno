using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Alumnos.Id;

[Authorize(Roles = Entrenador.RoleName)]
public class CrearPlanModel : EntrenadorPageModel
{
	public CrearPlanModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
		Id = string.Empty;
	}

	public Alumno Alumno { get; set; } = null!;
	public List<Ejercicio> Ejercicios { get; set; } = new();

	[BindProperty] public NuevoPlanForm Form { get; set; } = new();

	[FromRoute(Name = "id")] public string Id { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("Subscribe");

		var alumno = await DatabaseContext.Alumnos
			.FirstOrDefaultAsync(a => a.Id == Id && a.Entrenador == Entrenador);

		if (alumno == null) return RedirectToPage("Alumnos");

		Alumno = alumno;

		Ejercicios = await DatabaseContext.Ejercicios
			.Include(e => e.Categoria)
			.Where(e => e.Owner == Entrenador)
			.ToListAsync();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("Subscribe");

		var alumno = await DatabaseContext.Alumnos
			.FirstOrDefaultAsync(a => a.Id == Id && a.Entrenador == Entrenador);

		if (alumno == null) return RedirectToPage("Alumnos");
		Alumno = alumno;
		
		var ejerciciosAsignables = DatabaseContext.Ejercicios
			.Include(e => e.Categoria)
			.Where(e => e.Owner == Entrenador)
			.ToDictionary(e => e.Id);
		
		var plan = new JornadaEntrenamiento
		{
			Nombre = Form.Nombre,
			Descripcion = Form.Descripcion,
			FechaCreacion = DateTime.Now,
			FechaRealizacion = DateTime.Now,
			Ejercicios = new List<EjercicioProgramado>()
		};

		foreach (var ej in Form.Ejercicios)
		{
			plan.Ejercicios.Add(new EjercicioProgramado()
			{
				Ejercicio = ejerciciosAsignables[ej.Ejercicio],
				Series = ej.Series,
				Repeticiones = ej.Repeticiones
			});
		}
		
		Alumno.JornadasEntrenamientos.Add(plan);
		await DatabaseContext.SaveChangesAsync();

		return RedirectToPage("Index", new { id = Id });
	}
}

public class NuevoPlanForm
{
	public string Nombre { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
	public NuevoEjercicioPlan[] Ejercicios { get; set; }
}

public class NuevoEjercicioPlan
{
	public int Ejercicio { get; set; }
	public int Series { get; set; }
	public int Repeticiones { get; set; }
}