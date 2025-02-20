﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Areas.Dashboard.Services;

[Authorize(Roles = Entrenador.RoleName)]
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

	protected void LoadEntrenador(
		bool? includeCategorias = false
	)
	{
		if (_loaded) return;

		var user = UserManager.GetUserAsync(User).Result!;

		var ent = DatabaseContext.Entrenadores as IQueryable<Entrenador>;

		if (includeCategorias == true)
		{
			ent = ent.Include(e => e.Categorias);
		}

		Entrenador = ent.FirstOrDefault(e => e.Id == user.Id)!;

		_loaded = true;
	}
}