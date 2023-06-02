using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Ejercicios;

[Authorize(Roles = Entrenador.RoleName)]
public class EjerciciosNuevoModel : PageModel
{
	private readonly ApplicationDatabaseContext _context;
	private readonly UserManager<Usuario> _userManager;
	private readonly ILogger<EjerciciosNuevoModel> _logger;
	
	public EjerciciosNuevoModel(ApplicationDatabaseContext context, UserManager<Usuario> userManager, ILogger<EjerciciosNuevoModel> logger)
	{
		_context = context;
		_userManager = userManager;
		_logger = logger;
		Form = new NuevoEjercicioForm();
		
	}

	[BindProperty(Name = "cat", SupportsGet = true)]
	public string? CategoriaId { get; set; }
	
	[BindProperty]
	public NuevoEjercicioForm Form { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		if (string.IsNullOrEmpty(CategoriaId))
		{
			_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría.");
			return BadRequest();
		}
		
		var user = (await _userManager.GetUserAsync(User))!;
		var entrenador = await _context
			.Entrenadores
			.Include(e => e.Categorias)
			.FirstOrDefaultAsync(e => e.Id == user.Id);

		var categoria = entrenador!.Categorias.FirstOrDefault(c => c.Id.ToString() == CategoriaId);
		if (categoria == null)
		{
			_logger.LogError("Entrenador intentó crear un ejercicio en una categoría que no le pertenece.");
			return Forbid();
		};

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return Page();
		if (string.IsNullOrEmpty(CategoriaId))
		{
			_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría.");
			return BadRequest();
		};

		var user = (await _userManager.GetUserAsync(User))!;
		var entrenador = await _context
			.Entrenadores
			.Include(e => e.Categorias)
			.FirstOrDefaultAsync(e => e.Id == user.Id);

		var categoria = entrenador!.Categorias.FirstOrDefault(c => c.Id.ToString() == CategoriaId);
		if (categoria == null)
		{
			_logger.LogError("Entrenador intentó crear un ejercicio en una categoría que no le pertenece.");
			return Forbid();
		};
		
		var ejercicio = new Ejercicio
		{
			Nombre = Form.Nombre,
			Descripcion = Form.Descripcion,
			Owner = entrenador,
			Categoria = categoria,
			Dificultad = Form.Dificultad,
			VideoUrl = Form.UrlVideo
		};

		_context.Ejercicios.Add(ejercicio);
		await _context.SaveChangesAsync();
		
		return RedirectToPage("/Ejercicios/Index");
	}
}

public class NuevoEjercicioForm
{
	[Required] public string Nombre { get; set; } = string.Empty;

	[Required]
	public string Descripcion { get; set; } = string.Empty;
	
	public string? UrlVideo { get; set; }

	[Required] [Range(1, 5)] public byte Dificultad { get; set; } = 3;
}