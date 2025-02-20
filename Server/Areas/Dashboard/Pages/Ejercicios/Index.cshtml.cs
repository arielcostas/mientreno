﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages.Ejercicios;

[Authorize(Roles = Entrenador.RoleName)]
public class EjerciciosIndexModel : EntrenadorPageModel
{
	public EjerciciosIndexModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(userManager, databaseContext)
	{
		Categorias = new List<Categoria>();
		NuevaCategoriaName = string.Empty;
	}

	public List<Categoria> Categorias { get; set; }
	[BindProperty] public string NuevaCategoriaName { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		LoadEntrenador();

		Categorias = await DatabaseContext.Categorias
			.Include(c => c.Ejercicios)
			.Where(a => a.Owner.Id == Entrenador.Id)
			.ToListAsync();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadEntrenador();

		if (string.IsNullOrWhiteSpace(NuevaCategoriaName))
		{
			ModelState.AddModelError("NuevaCategoriaName", "El nombre de la categoría no puede estar vacío");
			return Page();
		}

		var categoria = new Categoria
		{
			Nombre = NuevaCategoriaName
		};
		
		Entrenador.Categorias.Add(categoria);
		await DatabaseContext.SaveChangesAsync();

		Categorias = await DatabaseContext.Categorias
			.Include(c => c.Ejercicios)
			.Where(a => a.Owner.Id == Entrenador.Id)
			.ToListAsync();
		NuevaCategoriaName = string.Empty;
		return Page();
	}
}