using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Data;

public class ApplicationDatabaseContext : IdentityDbContext
{
	public ApplicationDatabaseContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Tablas
		var usuario = modelBuilder.Entity<Usuario>();
		var entrenador = modelBuilder.Entity<Entrenador>();
		var cuestionario = modelBuilder.Entity<Cuestionario>();
		var ejercicio = modelBuilder.Entity<Ejercicio>();

		// Tabla común de usuarios con dos tipos de usuarios: Alumnos y Entrenadores.
		usuario.HasDiscriminator<string>("Tipo")
			.HasValue<Entrenador>("Entrenador")
			.HasValue<Alumno>("Alumno");

		// Entranador tiene varios alumnos
		entrenador
			.HasMany<Alumno>(e => e.Alumnos)
			.WithOne(a => a.Entrenador)
			.OnDelete(DeleteBehavior.Restrict);

		// Alumno tiene cuestionarios
		cuestionario
			.HasOne<Alumno>(c => c.Alumno)
			.WithMany(a => a.Cuestionarios);

		// Cuestionario tiene hábitos y perímetros.
		cuestionario.OwnsOne(c => c.Habitos);
		cuestionario.OwnsOne(c => c.Perimetros);

		// Ejercicios tienen categoría.
		ejercicio.HasOne(e => e.Categoria)
			.WithMany(c => c.Ejercicios)
			.OnDelete(DeleteBehavior.NoAction);

		// Un entrenador tiene ejercicios y categorías
		entrenador.HasMany(en => en.Ejercicios)
			.WithOne(ej => ej.Owner)
			.OnDelete(DeleteBehavior.Cascade);

		entrenador.HasMany(en => en.Categorias)
			.WithOne(c => c.Owner)
			.OnDelete(DeleteBehavior.Cascade);

		// Invitación del entrenador a un futuro alumno.
		entrenador.HasMany<Invitacion>(e => e.Invitaciones)
			.WithOne(i => i.Entrenador);
		
		// Alumno tiene rutinas
		var alumno = modelBuilder.Entity<Alumno>();
		alumno.HasMany<JornadaEntrenamiento>(a => a.JornadasEntrenamientos)
			.WithOne(j => j.ClienteAsignado)
			.OnDelete(DeleteBehavior.Cascade);

		// Jornada de entrenamiento tiene ejercicios
		var jornadaEntrenamiento = modelBuilder.Entity<JornadaEntrenamiento>();
		jornadaEntrenamiento.HasMany<EjercicioProgramado>(j => j.Ejercicios)
			.WithOne(e => e.Jornada)
			.OnDelete(DeleteBehavior.Cascade);

		// Ejercicio programado tiene un ejercicio
		var ejercicioProgramado = modelBuilder.Entity<EjercicioProgramado>();
		ejercicioProgramado.HasOne(e => e.Ejercicio)
			.WithMany()
			.OnDelete(DeleteBehavior.NoAction);
	}

	public required DbSet<Usuario> Usuarios { get; set; }
	public required DbSet<Entrenador> Entrenadores { get; set; }
	public required DbSet<Alumno> Alumnos { get; set; }
	public required DbSet<Invitacion> Invitaciones { get; set; }
	public required DbSet<Cuestionario> Cuestionarios { get; set; }
	public required DbSet<Ejercicio> Ejercicios { get; set; }
	public required DbSet<Categoria> Categorias { get; set; }
}