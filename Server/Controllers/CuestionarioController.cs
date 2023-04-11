using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Helpers;
using Mientreno.Server.Models;
using System.Security.Claims;

namespace Mientreno.Server.Controllers;

[ApiController]
[Authorize(Constantes.PolicyEsAlumno)]
public class CuestionarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public CuestionarioController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Devuelve todos los cuestionarios del alumno
    /// </summary>
    /// <returns></returns>
    [HttpGet("Cuestionarios")]
    public IActionResult Index()
    {
        var me = GetAlumnoAutenticado();

        if (me.AsignacionActual == null)
        {
            // TODO: Podría devolver el histórico o algo
            return NotFound();
        }
        var q = me.AsignacionActual.Cuestionarios;

        return Ok(q);
    }

    private Alumno GetAlumnoAutenticado()
    {
        var alumnoAllegedId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (alumnoAllegedId == null)
        {
            throw new Exception("No se ha podido obtener el id del alumno");
        }

        var alumno = _context.Alumnos
            .Include(a => a.Asignaciones)
            .FirstOrDefault(a => a.Id == Guid.Parse(alumnoAllegedId.Value));

        if (alumno == null)
        {
            throw new Exception("No se ha podido obtener el alumno");
        }

        return alumno;

    }
}
