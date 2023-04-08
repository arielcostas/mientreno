using System.ComponentModel.DataAnnotations;

namespace Mientreno.Server.Models;

/// <summary>
/// Un ejercicio es una actividad física que se realiza en un entrenamiento. Los ejercicios están guardados
/// bajo el "perfil" del entrenador, pudiendo usarse para varios entrenamientos de distintos alumnos. Cada 
/// ejercicio tiene opcionalmente una categoría, o puede estar sin categorizar (null).
/// </summary>
public class Ejercicio
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string? VideoUrl { get; set; }

    public Entrenador Owner { get; set; }
    public Categoria? Categoria { get; set; }

    [Range(1, 5)]
    public byte Dificultad { get; set; }
}

/// <summary>
/// Una categoría es un grupo de ejercicios relacionados. Por ejemplo, "Piernas" o "Brazos". están guardadas
/// bajo el "perfil" del entrenador, y solo pueden contener ejercicios de su propiedad.
/// </summary>
public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public Entrenador Owner { get; set; }
    public List<Ejercicio> Ejercicios { get; set; }
}