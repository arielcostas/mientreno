using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Accounts.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Accounts.Pages;

public class LogoutModel : UsuarioPageModel
{
	private readonly SignInManager<Usuario> _signInManager;

	public LogoutModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext, SignInManager<Usuario> signInManager) : base(userManager, databaseContext)
	{
		_signInManager = signInManager;
	}
	
	public async Task<IActionResult> OnGet()
	{
		await _signInManager.SignOutAsync();
		return RedirectToPage("/Login");
	}
}