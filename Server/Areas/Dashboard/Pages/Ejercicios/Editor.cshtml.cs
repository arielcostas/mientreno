using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

	public EjerciciosNuevoModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext,
		ILogger<EjerciciosNuevoModel> logger) : base(userManager, databaseContext)
	{
		_logger = logger;

		Form = new NuevoEjercicioForm();
	}

	[BindProperty(Name = "cat", SupportsGet = true)]
	public string? CategoriaId { get; set; }

	[BindProperty(Name = "id", SupportsGet = true)]
	public string? EditandoId { get; set; }

	[BindProperty] public NuevoEjercicioForm Form { get; set; }

	public bool Editando = false;

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador(includeCategorias: true);
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");

		if (!string.IsNullOrWhiteSpace(EditandoId))
		{
			return await OnEditEjercicio();
		}

		if (!string.IsNullOrWhiteSpace(CategoriaId))
		{
			return await OnNewEjercicio();
		}

		_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría.");
		return BadRequest();
	}

	private async Task<IActionResult> OnNewEjercicio()
	{
		var categoria = Entrenador.Categorias.FirstOrDefault(c => c.Id.ToString() == CategoriaId);
		if (categoria != null) return Page();

		_logger.LogError("Entrenador intentó crear un ejercicio en una categoría que no le pertenece.");
		return Forbid();
	}

	private async Task<IActionResult> OnEditEjercicio()
	{
		var ejercicio = await DatabaseContext.Ejercicios
			.Include(e => e.Categoria)
			.FirstOrDefaultAsync(e => e.Id.ToString() == EditandoId);

		if (ejercicio == null) return NotFound();

		if (ejercicio.Owner.Id != Entrenador.Id)
		{
			_logger.LogError("Entrenador intentó editar un ejercicio que no le pertenece.");
			return Forbid();
		}

		Form.Nombre = ejercicio.Nombre;
		Form.Descripcion = ejercicio.Descripcion;
		Form.Dificultad = ejercicio.Dificultad;
		Form.UrlVideo = ejercicio.VideoUrl;
		Editando = true;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadEntrenador(includeCategorias: true);
		if (!Entrenador.Suscripcion.Operativa) return RedirectToPage("/Subscribe");

		if (!ModelState.IsValid) return Page();
		
		if (!string.IsNullOrWhiteSpace(EditandoId))
		{
			return await OnEditingSave();
		}

		if (!string.IsNullOrEmpty(CategoriaId)) return await OnNewSave();
		
		_logger.LogError("Entrenador intentó crear un ejercicio sin especificar la categoría");
		return BadRequest();
	}

	private async Task<IActionResult> OnEditingSave()
	{
		var ejercicio = await DatabaseContext.Ejercicios
				.Include(e => e.Categoria)
				.FirstOrDefaultAsync(e => e.Id.ToString() == EditandoId) ?? new();
		
		if (ejercicio.Owner.Id != Entrenador.Id) return Forbid();
		
		ejercicio.Nombre = Form.Nombre;
		ejercicio.Descripcion = Form.Descripcion;
		ejercicio.Dificultad = Form.Dificultad;
		ejercicio.VideoUrl = Form.UrlVideo;
		
		await DatabaseContext.SaveChangesAsync();

		return RedirectToPage("/Ejercicios/Index");
	}

	private async Task<IActionResult> OnNewSave()
	{
		var categoria = Entrenador.Categorias.FirstOrDefault(c => c.Id.ToString() == CategoriaId);
		if (categoria == null)
		{
			_logger.LogError("Entrenador intentó crear un ejercicio en una categoría que no le pertenece");
			return Forbid();
		}

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

	[Required] public string Descripcion { get; set; } = string.Empty;

	public string? UrlVideo { get; set; }

	[Required] [Range(1, 5)] public byte Dificultad { get; set; } = 3;
}