using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Pages;

public class LoginChallengeModel : PageModel
{
	private readonly SignInManager<Usuario> _signInManager;
	private readonly UserManager<Usuario> _userManager;

	public LoginChallengeModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
	}

	[BindProperty] public LoginChallengeForm Form { get; set; } = new();
	
	public string MensajeError { get; set; } = string.Empty;

	[FromQuery]
	public string? ReturnUrl { get; set; }

	public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
	{
		// Ensure the user has gone through the username & password screen first
		var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

		if (user == null)
		{
			throw new InvalidOperationException($"Unable to load two-factor authentication user.");
		}

		ReturnUrl = returnUrl;

		return Page();
	}
	
	public async Task<IActionResult> OnPost(bool rememberMe, string? returnUrl = null)
	{
		if (!ModelState.IsValid) return Page();
		
		var usuario = _userManager.GetUserAsync(User).Result;
		if (usuario is not null)
		{
			return Redirect("/");
		}

		var code = Form.Code.Replace(" ", string.Empty)
			.Replace("-", string.Empty);
		
		var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
		
		if (user is null)
		{
			MensajeError = AppStrings.Error_IncorrectCredentials;
			return Page();
		}

		var res = await _signInManager.TwoFactorAuthenticatorSignInAsync(
			code,
			rememberMe,
			Form.RememberDevice
		);
		
		var area = usuario is Alumno ? "Alumnos" : "Dashboard";
		ReturnUrl ??= Url.Content($"/{area}");
		if (res.Succeeded) return Redirect(ReturnUrl);

		res = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);
		if (res.Succeeded) return Redirect(ReturnUrl);
		
		MensajeError = AppStrings.Error_IncorrectMfaCode;
		return Page();
	}
}

public class LoginChallengeForm
{
	public string Code { get; set; } = string.Empty;
	public bool RememberDevice { get; set; } = false;
}