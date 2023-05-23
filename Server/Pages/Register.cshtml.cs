using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Models;

namespace Mientreno.Server.Pages;

public class RegisterModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;

	public RegisterModel(UserManager<Usuario> userManager)
	{
		_userManager = userManager;
	}

	[BindProperty] public RegisterForm Form { get; set; } = new();

	public async Task<IActionResult> OnPost()
	{
		Entrenador nuevo = new()
		{
			Nombre = "Ariel",
			Apellidos = "Costas",
			FechaAlta = DateTime.Now
		};

		await _userManager.SetUserNameAsync(nuevo, "ArielCostas");
		await _userManager.SetEmailAsync(nuevo, "arielcostas@gmail.com");
		var result = await _userManager.CreateAsync(nuevo, "abc123");

		if (result.Succeeded)
		{
			return RedirectToPage("/Login");
		}
		
		foreach (var error in result.Errors)
		{
			Console.WriteLine(@$"{error.Code}: {error.Description}");
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}
}

public class RegisterForm
{
	[Required] [MinLength(2)] public string Nombre { get; set; } = string.Empty;

	[Required] [MinLength(2)] public string Apellidos { get; set; } = string.Empty;

	[Required] [MinLength(2)] public string Email { get; set; } = string.Empty;

	[Required] public string Contraseña { get; set; } = string.Empty;

	[Required]
	[Compare(nameof(Contraseña),
		ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.passwordsDoNotMatch)
	)]
	public string ConfirmarContraseña { get; set; } = string.Empty;

	[Required] public bool AceptoTerminos { get; set; } = false;
}