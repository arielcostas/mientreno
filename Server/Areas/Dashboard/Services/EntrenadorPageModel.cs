using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Services;

public class EntrenadorPageModel : PageModel
{
	
	protected readonly UserManager<Usuario> UserManager;
	protected readonly ApplicationDatabaseContext DatabaseContext;
	
	public EntrenadorPageModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext)
	{
		UserManager = userManager;
		DatabaseContext = databaseContext;
		Entrenador = null!;
	}
	
	private bool _loaded = false;
	public Entrenador Entrenador;
	
	protected void LoadEntrenador()
	{
		if (_loaded) return;
		
		var user = UserManager.GetUserAsync(User).Result!;
		
		Entrenador = DatabaseContext.Entrenadores
			.Include(e => e.Suscripcion)
			.FirstOrDefault(e => e.Id == user.Id)!;
		
		_loaded = true;
	}
	
	
}