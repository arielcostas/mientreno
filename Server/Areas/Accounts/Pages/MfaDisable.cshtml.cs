using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Areas.Accounts.Services;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Net.Codecrete.QrCodeGenerator;

namespace Mientreno.Server.Areas.Accounts.Pages;

public class MfaDisableModel : UsuarioPageModel
{
	public MfaDisableModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}
	
	public async Task<IActionResult> OnGetAsync()
	{
		LoadUsuario();
		
		if (!await UserManager.GetTwoFactorEnabledAsync(Usuario))
		{
			return RedirectToPage("/Index");
		}
		
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadUsuario();
		
		if (!await UserManager.GetTwoFactorEnabledAsync(Usuario))
		{
			return RedirectToPage("/Index");
		}

		if (!await UserManager.CheckPasswordAsync(Usuario, Password))
		{
			ModelState.AddModelError(nameof(Password), AppStrings.Error_IncorrectPassword);
			return Page();
		}
		
		await UserManager.SetTwoFactorEnabledAsync(Usuario, false);
		await UserManager.ResetAuthenticatorKeyAsync(Usuario);

		return RedirectToPage("/Index");
	}
	
	[BindProperty] public string Password { get; set; } = string.Empty;
}