using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Accounts.Services;

[Authorize]
public class UsuarioPageModel : PageModel
{
	protected readonly UserManager<Usuario> UserManager;
	protected readonly ApplicationDatabaseContext DatabaseContext;

	public UsuarioPageModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext)
	{
		UserManager = userManager;
		DatabaseContext = databaseContext;
		Usuario = null!;
	}

	private bool _loaded;
	public Usuario Usuario;

	protected void LoadUsuario()
	{
		if (_loaded) return;

		var user = UserManager.GetUserAsync(User).Result!;

		var usuario = DatabaseContext.Usuarios;

		Usuario = usuario.AsSplitQuery().FirstOrDefault(e => e.Id == user.Id)!;

		_loaded = true;
	}
}