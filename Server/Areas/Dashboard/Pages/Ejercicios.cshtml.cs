using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class EjerciciosModel : PageModel
{
	private readonly ApplicationContext _context;
	private readonly UserManager<Usuario> _userManager;

	public EjerciciosModel(ApplicationContext context, UserManager<Usuario> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public List<Categoria> Categorias { get; set; }
	[BindProperty] public string NuevaCategoriaName { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;

		Categorias = await _context.Categorias
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
		var e2 = (await _context.Entrenadores.Include(e => e.Categorias).FirstOrDefaultAsync(e => e.Id == entrenador.Id))!;

		var categoria = new Categoria
		{
			Nombre = NuevaCategoriaName
		};
		
		e2.Categorias.Add(categoria);
		await _context.SaveChangesAsync();

		Categorias = await _context.Categorias
			.Include(c => c.Ejercicios)
			.Where(a => a.Owner.Id == entrenador.Id)
			.ToListAsync();
		return Page();
	}
}