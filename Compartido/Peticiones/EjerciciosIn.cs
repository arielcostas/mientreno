﻿using System.ComponentModel.DataAnnotations;

namespace Mientreno.Compartido.Peticiones;

public class NuevoEjercicioInput
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? VideoUrl { get; set; } = string.Empty;
    public int? IdCategoria { get; set; }

    [Range(1,5)]
    public int Dificultad { get; set; } = 1;
}

public class ActualizarEjercicioInput
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? VideoUrl { get; set; }
    public int? IdCategoria { get; set; }

    [Range(1, 5)]
    public int? Dificultad { get; set; }
}
