using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Ejercicios;

[Authorize(Roles = Entrenador.RoleName)]
public class EjerciciosNuevoModel : EntrenadorPageModel
{
	private readonly ILogger<EjerciciosNuevoModel> _logger;

	public EjerciciosNuevoModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext, ILogger<EjerciciosNuevoModel> logger) : base(userManager, databaseContext)
	{
		_logger = logger;
		
		Form = new NuevoEjercicioForm();
	}

	[BindProperty(Name = "cat", SupportsGet = true)]
	public string? CategoriaId { get; set; }
	
	[BindProperty]
	public NuevoEjercicioForm Form { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("Subscribe");

		if (string.IsNullOrEmpty(CategoriaId))
		{
			_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría.");
			return BadRequest();
		}
		
		var user = (await UserManager.GetUserAsync(User))!;
		var entrenador = await DatabaseContext
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
		LoadEntrenador();
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");

		if (!ModelState.IsValid) return Page();
		if (string.IsNullOrEmpty(CategoriaId))
		{
			_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría.");
			return BadRequest();
		};
		
		var categoria = Entrenador.Categorias.FirstOrDefault(c => c.Id.ToString() == CategoriaId);
		if (categoria == null)
		{
			_logger.LogError("Entrenador intentó crear un ejercicio en una categoría que no le pertenece.");
			return Forbid();
		};
		
		var ejercicio = new Ejercicio
		{
			Nombre = Form.Nombre,
			Descripcion = Form.Descripcion,
			Owner = Entrenador,
			Categoria = categoria,
			Dificultad = Form.Dificultad,
			VideoUrl = Form.UrlVideo
		};

		DatabaseContext.Ejercicios.Add(ejercicio);
		await DatabaseContext.SaveChangesAsync();
		
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