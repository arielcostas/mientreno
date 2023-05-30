using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

[Authorize(Roles = Entrenador.RoleName)]
public class AlumnosModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;
	private readonly ApplicationDatabaseContext _context;

	public AlumnosModel(UserManager<Usuario> userManager, ApplicationDatabaseContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public Alumno[] Alumnos { get; set; } = Array.Empty<Alumno>();
	public NuevaInvitacionForm Form { get; set; } = new();
	public string? EnlaceInvitacion { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Alumnos = _context.Alumnos
			.Where(a => a.Entrenador == entrenador)
			.ToArray();

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
			MaximoUsos = byte.Parse(Request.Form["cantidad"].FirstOrDefault() ?? "0")
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