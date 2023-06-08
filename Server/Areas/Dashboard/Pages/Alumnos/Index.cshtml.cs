﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service;
using Stripe;

namespace Mientreno.Server.Areas.Dashboard.Pages.Alumnos;

[Authorize(Roles = Entrenador.RoleName)]
public class AlumnosModel : EntrenadorPageModel
{
	public AlumnosModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(userManager, databaseContext)
	{
	}

	public Alumno[] Alumnos { get; set; } = Array.Empty<Alumno>();
	public string? EnlaceInvitacion { get; set; }
	public int MaxInvitaciones;

	public IActionResult OnGet()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");
		
		Alumnos = DatabaseContext.Alumnos
			.Where(a => a.Entrenador == Entrenador)
			.ToArray();

		MaxInvitaciones = SubscriptionRestrictions.MaxAlumnosPerEntrenador(Entrenador.Suscripcion.Plan) -
		                  Alumnos.Length;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");
		
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