using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service;

namespace Mientreno.Server.Areas.Dashboard.Pages.Alumnos.Id;

[Authorize(Roles = Entrenador.RoleName)]
public class PlanEditorModel : EntrenadorPageModel
{
	public PlanEditorModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
		AlumnoId = string.Empty;
		Alumno = null!;
		Ejercicios = new List<Ejercicio>();
		Form = new NuevoPlanForm();
	}

	public Alumno Alumno { get; set; }
	public List<Ejercicio> Ejercicios { get; set; }

	[BindProperty] public NuevoPlanForm Form { get; set; }
	public bool Editable = true;

	[FromRoute(Name = "id")] public string AlumnoId { get; set; }
	[BindProperty(Name = "p", SupportsGet = true)] public string? PlanId { get; set; }
	
	public Feedback? Feedback { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");

		var alumno = await DatabaseContext.Alumnos
			.Include(a => a.JornadasEntrenamientos)
			.ThenInclude(j => j.Ejercicios)
			.ThenInclude(e => e.Ejercicio)
			.AsSplitQuery()
			.FirstOrDefaultAsync(a => a.Id == AlumnoId && a.Entrenador == Entrenador);
		if (alumno == null) return RedirectToPage("Alumnos");
		Alumno = alumno;

		if (PlanId.IsNullOrWhiteSpace())
		{
			JornadaEntrenamiento nuevoPlan = new()
			{
				Nombre = string.Empty,
				Descripcion = string.Empty,
				ClienteAsignado = alumno,
				Ejercicios = new List<EjercicioProgramado>(),
				FechaCreacion = DateTime.Now,
			};
			
			alumno.JornadasEntrenamientos.Add(nuevoPlan);
			await DatabaseContext.SaveChangesAsync();
			
			return RedirectToPage("Plan", new { id = AlumnoId, p = nuevoPlan.Id });
		}
		
		var plan = Alumno.JornadasEntrenamientos
			.FirstOrDefault(j => j.Id == PlanId && j.ClienteAsignado.Id == alumno.Id);

		if (plan is null) return NotFound();

		Form = new NuevoPlanForm()
		{
			Nombre = plan.Nombre,
			Descripcion = plan.Descripcion,
			EjerciciosPlan = plan.Ejercicios.Select(e => new NuevoEjercicioPlan()
			{
				Ejercicio = e.Ejercicio.Id,
				Series = e.Series,
				Repeticiones = e.Repeticiones,
				Minutos = e.Minutos
			}).ToArray(),
			Publicar = plan.Estado != EstadoRutina.Borrador
		};
		
		Editable = plan.Estado == EstadoRutina.Borrador;

		Ejercicios = await DatabaseContext.Ejercicios
			.Include(e => e.Categoria)
			.Where(e => e.Owner == Entrenador)
			.ToListAsync();

		if (plan.Estado == EstadoRutina.Evaluada)
		{
			Feedback = new Feedback()
			{
				Valoracion = plan.Valoracion,
				Comentario = plan.Comentario
			};
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");

		if (PlanId.IsNullOrWhiteSpace()) return BadRequest();
		
		var alumno = await DatabaseContext.Alumnos
			.Include(a => a.JornadasEntrenamientos)
			.ThenInclude(j => j.Ejercicios)
			.AsSplitQuery()
			.FirstOrDefaultAsync(a => a.Id == AlumnoId && a.Entrenador == Entrenador);

		if (alumno == null) return RedirectToPage("Alumnos");
		Alumno = alumno;
		
		var plan = Alumno.JornadasEntrenamientos
			.FirstOrDefault(j => j.Id == PlanId && j.ClienteAsignado.Id == alumno.Id);

		if (plan is null) return NotFound();
		
		var ejerciciosAsignables = DatabaseContext.Ejercicios
			.Include(e => e.Categoria)
			.Where(e => e.Owner == Entrenador)
			.ToDictionary(e => e.Id);
		
		plan.Nombre = Form.Nombre;
		plan.Descripcion = Form.Descripcion;
		plan.Ejercicios.Clear();
		
		if (Form.Publicar) plan.FechaPublicacion = DateTime.Now;

		foreach (var ej in Form.EjerciciosPlan)
		{
			plan.Ejercicios.Add(new EjercicioProgramado()
			{
				Ejercicio = ejerciciosAsignables[ej.Ejercicio],
				Series = ej.Series,
				Repeticiones = ej.Repeticiones,
				Minutos = ej.Minutos
			});
		}
		
		await DatabaseContext.SaveChangesAsync();

		return RedirectToPage("Index", new { id = AlumnoId });
	}
}

public class Feedback
{
	public byte Valoracion { get; set; }
	public string Comentario { get; set; } = string.Empty;
}

public class NuevoPlanForm
{
	public string Nombre { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
	public NuevoEjercicioPlan[] EjerciciosPlan { get; set; } = Array.Empty<NuevoEjercicioPlan>();
	public bool Publicar { get; set; } = false;
}

public class NuevoEjercicioPlan
{
	[Required]
	public int Ejercicio { get; set; }
	
	[Range(0, int.MaxValue)]
	public int? Series { get; set; }
	
	[Range(0, int.MaxValue)]
	public int? Repeticiones { get; set; }
	
	[Range(0, int.MaxValue)]
	public int? Minutos { get; set; }
}