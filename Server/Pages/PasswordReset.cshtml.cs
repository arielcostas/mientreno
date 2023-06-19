using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Pages;

public class PasswordResetModel : PageModel
{
	private readonly ILogger<PasswordResetModel> _logger;
	private readonly UserManager<Usuario> _userManager;

	public PasswordResetModel(UserManager<Usuario> userManager, ILogger<PasswordResetModel> logger)
	{
		_userManager = userManager;
		_logger = logger;
		Token = string.Empty;
	}

	[BindProperty] public PasswordResetForm Form { get; set; } = new();

	[BindProperty(SupportsGet = true, Name = "code")]
	public string Token { get; set; }

	[BindProperty(SupportsGet = true)] public string? Email { get; set; } = string.Empty;

	public IActionResult OnGetAsync()
	{
		Form.Token = Token;
		Form.Email = Email ?? string.Empty;

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

		var user = await _userManager.FindByEmailAsync(Form.Email);
		if (user is null)
		{
			ModelState.AddModelError(nameof(Form.Email), AppStrings.Error_InvalidResetToken);
			return Page();
		}

		var result = await _userManager.ResetPasswordAsync(user, Form.Token, Form.Contraseña);

		if (result.Succeeded)
		{
			_logger.LogInformation("PasswordReset: Password reset for {Email}", Form.Email);

			return RedirectToPage("/Login");
		}

		foreach (var error in result.Errors)
		{
			switch (error.Code)
			{
				case "InvalidToken":
					_logger.LogWarning("PasswordReset: Invalid token for {Email}", Form.Email);
					ModelState.AddModelError(nameof(Form.Email), AppStrings.Error_InvalidResetToken);
					break;
				case "PasswordRequiresDigit" or "PasswordRequiresLower" or "PasswordRequiresUpper"
					or "PasswordRequiresNonAlphanumeric" or "PasswordTooShort":
					_logger.LogWarning("PasswordReset: Invalid password for {Email}", Form.Email);
					ModelState.AddModelError(nameof(Form.Contraseña), AppStrings.Validation_Password_Weak);
					break;
			}
		}

		return Page();
	}
}

public class PasswordResetForm
{
	[Required] public string Token { get; set; } = string.Empty;

	[Required] public string Email { get; set; } = string.Empty;

	[Required] public string Contraseña { get; set; } = string.Empty;

	[Required]
	[Compare(nameof(Contraseña))]
	public string RepetirContraseña { get; set; } = string.Empty;
}