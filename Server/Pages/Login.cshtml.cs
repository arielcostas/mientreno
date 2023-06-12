using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Pages;

public class LoginModel : PageModel
{
	private readonly SignInManager<Usuario> _signInManager;
	private readonly UserManager<Usuario> _userManager;

	public LoginModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
	}

	[BindProperty] public LoginForm Form { get; set; } = new();

	[BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

	public IActionResult OnGet()
	{
		var usuario = _userManager.GetUserAsync(User).Result;
		if (usuario is not null)
		{
			var area = usuario is Alumno ? "Alumnos" : "Dashboard";
			return Redirect($"/{area}");
		}

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		var usuario = _userManager.GetUserAsync(User).Result;
		if (usuario is not null)
		{
			var area = usuario is Alumno ? "Alumnos" : "Dashboard";
			return Redirect($"/{area}");
		}

		if (!ModelState.IsValid) return Page();

		var user = await _userManager.FindByEmailAsync(Form.Email);

		if (user is null)
		{
			ModelState.AddModelError(nameof(Form.Contraseña), AppStrings.Error_IncorrectCredentials);
			return Page();
		}

		var res = await _signInManager.PasswordSignInAsync(
			user,
			Form.Contraseña,
			Form.Recordar,
			false
		);

		if (res.Succeeded) return Redirect(ReturnUrl ?? "/Index");
		if (res.RequiresTwoFactor)
			return RedirectToPage("/LoginChallenge", new
			{
				ReturnUrl,
				RememberMe = Form.Recordar
			});

		ModelState.AddModelError(nameof(Form.Contraseña), AppStrings.Error_IncorrectCredentials);
		return Page();
	}
}

public class LoginForm
{
	[Required(ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.Validation_Email_Required))]
	[EmailAddress(ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.Validation_Email_Required))]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.Validation_Password_Required))]
	public string Contraseña { get; set; } = string.Empty;
	
	public bool Recordar { get; set; } = false;
}