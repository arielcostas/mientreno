using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Areas.Alumnos.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service;

namespace Mientreno.Server.Areas.Alumnos.Pages;

public class AlumnoPlanModel : AlumnoPageModel
{
	public AlumnoPlanModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}

	[BindProperty(SupportsGet = true, Name = "p")]
	public string PlanId { get; set; } = string.Empty;

	public JornadaEntrenamiento Plan { get; set; } = new();

	public IActionResult OnGet()
	{
		if (PlanId.IsNullOrWhiteSpace()) return BadRequest();

		LoadAlumno(includeJornadas: true);

		var plan = Alumno.JornadasEntrenamientos
			.FirstOrDefault(x => x.Id == PlanId);

		if (plan is null || plan.Estado == EstadoRutina.Borrador) return NotFound();
		Plan = plan;

		return Page();
	}


	[BindProperty] public PlanFeedback Feedback { get; set; } = new();

	public IActionResult OnPost()
	{
		if (PlanId.IsNullOrWhiteSpace()) return BadRequest();

		LoadAlumno(includeJornadas: true);

		var plan = Alumno.JornadasEntrenamientos
			.FirstOrDefault(x => x.Id == PlanId);

		if (plan is null || plan.Estado == EstadoRutina.Borrador) return NotFound();

		switch (plan.Estado)
		{
			case EstadoRutina.Publicada:
				plan.FechaInicioRealizacion = DateTime.Now;
				break;
			case EstadoRutina.EnCurso:
				plan.FechaFinRealizacion = DateTime.Now;
				break;
			case EstadoRutina.Finalizada:
				plan.FechaEvalucion = DateTime.Now;
				plan.Comentario = Feedback.Comentario;
				plan.Valoracion = Feedback.Puntuacion;
				break;
			case EstadoRutina.Evaluada:
				return RedirectToPage("Index");
		}

		DatabaseContext.SaveChanges();

		return RedirectToPage("Plan", null, new { p = PlanId });
	}
}

public class PlanFeedback
{
	public string Comentario { get; set; } = string.Empty;

	[Range(1, 5)] public byte Puntuacion { get; set; } = 3;
}