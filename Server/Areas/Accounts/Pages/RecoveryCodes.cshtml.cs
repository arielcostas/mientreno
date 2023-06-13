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

public class GenerateRecoveryCodesModel : UsuarioPageModel
{
	public GenerateRecoveryCodesModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}
	
	public async Task<IActionResult> OnGetAsync()
	{
		LoadUsuario();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadUsuario();

		if (!await UserManager.CheckPasswordAsync(Usuario, Password))
		{
			ModelState.AddModelError(nameof(Password), AppStrings.Error_IncorrectPassword);
			return Page();
		}
		
		Password = string.Empty;
		var codes = await UserManager
			.GenerateNewTwoFactorRecoveryCodesAsync(Usuario, 8);
		
		Codes = codes!.ToList();

		return Page();
	}
	
	[BindProperty] public string Password { get; set; } = string.Empty;
	public List<string>? Codes { get; set; }
}