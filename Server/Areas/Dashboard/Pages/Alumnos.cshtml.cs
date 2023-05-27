using Microsoft.AspNetCore.Identity;
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
	
	public async Task<IActionResult> OnGetAsync()
	{
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Alumnos = entrenador.Alumnos.ToArray();

		return Page();
	}
}