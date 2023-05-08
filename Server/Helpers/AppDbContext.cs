using Microsoft.EntityFrameworkCore;
using Mientreno.Server.Models;

namespace Mientreno.Server.Helpers;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options)
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
		var categoria = modelBuilder.Entity<Categoria>();

		// Tabla común de usuarios con dos tipos de usuarios: Alumnos y Entrenadores.
		usuario.HasDiscriminator<string>("Tipo")
			.HasValue<Entrenador>("Entrenador")
			.HasValue<Alumno>("Alumno");

		usuario.HasMany(u => u.Sesiones)
			.WithOne(s => s.Usuario)
			.OnDelete(DeleteBehavior.Cascade);

		// Credenciales de usuario.
		usuario.OwnsOne(u => u.Credenciales);

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
	}

	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<Entrenador> Entrenadores { get; set; }
	public DbSet<Alumno> Alumnos { get; set; }
	public DbSet<Sesion> Sesiones { get; set; }

	public DbSet<Cuestionario> Cuestionarios { get; set; }
}