﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class AlumnosModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;

	public AlumnosModel(UserManager<Usuario> userManager)
	{
		_userManager = userManager;
	}

	public Alumno[] Alumnos { get; set; } = Array.Empty<Alumno>();
	public NuevaInvitacionForm Form { get; set; } = new();
	public string? EnlaceInvitacion { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Alumnos = entrenador.Alumnos.ToArray();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();

		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Invitacion invitacion = new()
		{
			Entrenador = entrenador,
			Usos = 0,
			MaximoUsos = (byte)Form.Cantidad
		};

		entrenador.Invitaciones.Add(invitacion);
		await _userManager.UpdateAsync(entrenador);

		EnlaceInvitacion = Url.Page(
			"/Register",
			null,
			new { area = "", inv = invitacion.Id },
			Request.Scheme
		);

		return Page();
	}
}

public class NuevaInvitacionForm
{
	public int Cantidad { get; set; }
}