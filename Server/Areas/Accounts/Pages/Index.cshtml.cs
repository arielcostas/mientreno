using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Accounts.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Accounts.Pages;

public class AccountIndexModel : UsuarioPageModel
{
	public AccountIndexModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}

	public IActionResult OnGet()
	{
		LoadUsuario();
		return Page();
	}
}