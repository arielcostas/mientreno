﻿using Mientreno.Compartido.Recursos;
using Mientreno.Server.Extensions;

namespace Mientreno.Server.Data.Models;

public class JornadaEntrenamiento
{
	public string Id { get; set; } = Random.Shared.NextString(12);
	
	public string Nombre { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
	public Alumno ClienteAsignado { get; set; } = new();
	public List<EjercicioProgramado> Ejercicios { get; set; } = new();
	
	public DateTime FechaCreacion { get; set; }
	public DateTime? FechaPublicacion { get; set; }
	public DateTime? FechaInicioRealizacion { get; set; }
	public DateTime? FechaFinRealizacion { get; set; }
	public DateTime? FechaEvalucion { get; set; }
	
	public byte Valoracion { get; set; }
	public string Comentario { get; set; } = string.Empty;

	public EstadoRutina Estado => GetEstado();

	private EstadoRutina GetEstado()
	{
		if (FechaEvalucion is not null) return EstadoRutina.Evaluada;
		if (FechaFinRealizacion is not null) return EstadoRutina.Finalizada;
		if (FechaInicioRealizacion is not null) return EstadoRutina.EnCurso;
		return FechaPublicacion is not null ? EstadoRutina.Publicada : EstadoRutina.Borrador;
	}
	
	public string BadgeColour => GetBadgeColour();

	private string GetBadgeColour()
	{
		return Estado switch
		{
			EstadoRutina.Borrador => "bad",
			EstadoRutina.Publicada => "warning",
			EstadoRutina.EnCurso => "warning",
			EstadoRutina.Finalizada => "good",
			EstadoRutina.Evaluada => "good",
			_ => "bad"
		};
	}
	
	public string BadgeText => GetBadgeText();

	private string GetBadgeText()
	{
		return Estado switch
		{
			EstadoRutina.Borrador => AppStrings.Noun_EstadoRutina_Borrador,
			EstadoRutina.Publicada => AppStrings.Noun_EstadoRutina_Publicada,
			EstadoRutina.EnCurso => AppStrings.Noun_EstadoRutina_EnCurso,
			EstadoRutina.Finalizada => AppStrings.Noun_EstadoRutina_Finalizada,
			EstadoRutina.Evaluada => AppStrings.Noun_EstadoRutina_Evaluada,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
	
	public DateTime UltimaFecha => GetUltimaFecha();

	private DateTime GetUltimaFecha()
	{
		return FechaEvalucion ??
		       FechaFinRealizacion ??
		       FechaInicioRealizacion ??
		       FechaPublicacion ??
		       FechaCreacion;
	}
}

public enum EstadoRutina
{
	Borrador,
	Publicada,
	EnCurso,
	Finalizada,
	Evaluada
}

public class EjercicioProgramado
{
	public int Id { get; set; }
	public JornadaEntrenamiento Jornada { get; set; } = new();
	
	public Ejercicio Ejercicio { get; set; } = new();

	public int? Series { get; set; }
	public int? Repeticiones { get; set; }
	public int? Minutos { get; set; }
}
