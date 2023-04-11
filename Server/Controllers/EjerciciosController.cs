using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Compartido.Peticiones;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;
using System.Collections;
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
    public ActionResult<List<MiEjercicio>> MisEjercicios()
    {
        var me = GetEntrenadorAutenticado();
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
    public ActionResult NuevoEjercicio([FromBody] NuevoEjercicioInput input)
    {
        var me = GetEntrenadorAutenticado();

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

        return NoContent();
    }

    private Entrenador GetEntrenadorAutenticado()
    {
        var entrenadorAllegedId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (entrenadorAllegedId == null)
        {
            throw new Exception("No se ha podido obtener el id del entrenador");
        }

        var entrenador = _context.Entrenadores
            .Include(e => e.Ejercicios)
            .FirstOrDefault(e => e.Id == Guid.Parse(entrenadorAllegedId.Value));

        if (entrenador == null)
        {
            throw new Exception("No se ha podido obtener el entrenador");
        }

        return entrenador;

    }
}
