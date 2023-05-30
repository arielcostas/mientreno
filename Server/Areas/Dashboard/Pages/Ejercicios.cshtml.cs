using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service;

namespace Mientreno.Server.Areas.Dashboard.Pages;

[Authorize(Roles = Entrenador.RoleName)]
public class EjerciciosModel : PageModel
{
	private readonly ApplicationDatabaseContext _databaseContext;
	private readonly UserManager<Usuario> _userManager;

	public EjerciciosModel(ApplicationDatabaseContext databaseContext, UserManager<Usuario> userManager)
	{
		_databaseContext = databaseContext;
		_userManager = userManager;

		Categorias = new List<Categoria>();
		NuevaCategoriaName = string.Empty;
	}

	public List<Categoria> Categorias { get; set; }
	[BindProperty] public string NuevaCategoriaName { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Categorias = await _databaseContext.Categorias
			.Include(c => c.Ejercicios)
			.Where(a => a.Owner.Id == entrenador.Id)
			.ToListAsync();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrWhiteSpace(NuevaCategoriaName))
		{
			return Page();
		}
		
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;
		var e2 = (await _databaseContext.Entrenadores.Include(e => e.Categorias).FirstOrDefaultAsync(e => e.Id == entrenador.Id))!;

		var categoria = new Categoria
		{
			Nombre = NuevaCategoriaName
		};
		
		e2.Categorias.Add(categoria);
		await _databaseContext.SaveChangesAsync();

		Categorias = await _databaseContext.Categorias
			.Include(c => c.Ejercicios)
			.Where(a => a.Owner.Id == entrenador.Id)
			.ToListAsync();
		return Page();
	}
}