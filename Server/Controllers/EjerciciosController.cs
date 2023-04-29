using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;
using System.Security.Claims;

namespace Mientreno.Server.Controllers;

[ApiController]
[Authorize(Constantes.PolicyEsEntrenador)]
public class EjerciciosController : ControllerBase
{
	private readonly AppDbContext _context;

	public EjerciciosController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet("Ejercicios")]
	public async Task<ActionResult<List<MiEjercicio>>> MisEjercicios()
	{
		var me = await GetEntrenadorAutenticado();
		return Ok(me.Ejercicios.Select(e => new MiEjercicio
		{
			Id = e.Id,
			Nombre = e.Nombre,
			Descripcion = e.Descripcion,
			VideoUrl = e.VideoUrl,
			Dificultad = e.Dificultad,
			Categoria = (e.Categoria != null) ? new()
			{
				Id = e.Categoria.Id,
				Nombre = e.Categoria.Nombre
			} : null
		}));
	}

	[HttpPost("Ejercicios")]
	public async Task<ActionResult<NuevoEjercicioOutput>> NuevoEjercicio([FromBody] NuevoEjercicioInput input)
	{
		var me = await GetEntrenadorAutenticado();

		Categoria? categoria = null;
		if (input.IdCategoria != null)
		{
			categoria = me.Categorias.FirstOrDefault(c => c.Id == input.IdCategoria);
		}

		Ejercicio nuevoEjercicio = new()
		{
			Nombre = input.Nombre,
			Descripcion = input.Descripcion,
			VideoUrl = input.VideoUrl,
			Dificultad = (byte)input.Dificultad,
			Categoria = categoria
		};

		me.Ejercicios.Add(nuevoEjercicio);
		_context.SaveChanges();

		return Ok(new NuevoEjercicioOutput() { Id = nuevoEjercicio.Id });
	}

	[HttpPatch]
	[Route("Ejercicios/{id}")]
	public async Task<ActionResult<MiEjercicio>> ActualizarEjercicio(int ejercicioId, [FromBody] ActualizarEjercicioInput input)
	{
		var me = await GetEntrenadorAutenticado();
		var ejercicio = me.Ejercicios.FirstOrDefault(e => e.Id == ejercicioId);
		if (ejercicio == null)
		{
			return NotFound();
		}

		Categoria? categoria = null;
		if (input.IdCategoria != null)
		{
			categoria = me.Categorias.FirstOrDefault(c => c.Id == input.IdCategoria);
		}

		ejercicio.Nombre = input.Nombre ?? ejercicio.Nombre;
		ejercicio.Descripcion = input.Descripcion ?? ejercicio.Descripcion;
		ejercicio.VideoUrl = input.VideoUrl ?? ejercicio.VideoUrl;
		ejercicio.Dificultad = (byte)(input.Dificultad ?? ejercicio.Dificultad);
		ejercicio.Categoria = categoria ?? ejercicio.Categoria;

		await _context.SaveChangesAsync();

		return Ok(new MiEjercicio
		{
			Id = ejercicio.Id,
			Nombre = ejercicio.Nombre,
			Descripcion = ejercicio.Descripcion,
			VideoUrl = ejercicio.VideoUrl,
			Dificultad = ejercicio.Dificultad,
			Categoria = (ejercicio.Categoria != null) ? new()
			{
				Id = ejercicio.Categoria.Id,
				Nombre = ejercicio.Categoria.Nombre
			} : null
		});
	}

	[HttpDelete("Ejercicios/{id}")]
	public async Task<ActionResult> Eliminar(int id)
	{
		var me = await GetEntrenadorAutenticado();
		var ej = me.Ejercicios.FirstOrDefault(e => e.Id == id);

		if (ej == null)
		{
			return NotFound();
		}

		_context.Remove(ej);
		await _context.SaveChangesAsync();

		return NoContent();
	}

	private async Task<Entrenador> GetEntrenadorAutenticado()
	{
		var entrenadorAllegedId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

		if (entrenadorAllegedId == null)
		{
			throw new Exception("No se ha podido obtener el id del entrenador");
		}

		var entrenador = await _context.Entrenadores
			.Include(e => e.Ejercicios)
			.FirstOrDefaultAsync(e => e.Id == Guid.Parse(entrenadorAllegedId.Value));

		if (entrenador == null)
		{
			throw new Exception("No se ha podido obtener el entrenador");
		}

		return entrenador;
	}
}
