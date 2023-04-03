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

        // Tabla común de usuarios con dos tipos de usuarios: Alumnos y Entrenadores.
        var usuario = modelBuilder.Entity<Usuario>();
        usuario.HasDiscriminator<string>("Tipo")
            .HasValue<Entrenador>("Entrenador")
            .HasValue<Alumno>("Alumno");

        usuario.HasMany<Sesion>()
            .WithOne(s => s.Usuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Credenciales de usuario.
        usuario.OwnsOne(u => u.Credenciales);

        // Asignacion a entrenador.
        var contrato = modelBuilder.Entity<Contrato>();
        contrato.HasOne(a => a.Entrenador)
            .WithMany(e => e.Asignaciones)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        // Asignacion a alumno.
        contrato
            .HasOne(a => a.Alumno)
            .WithMany(a => a.Asignaciones)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        // Cuestionario tiene hábitos y perímetros.
        var cuestionario = modelBuilder.Entity<Cuestionario>();
        cuestionario.OwnsOne(c => c.Habitos);
        cuestionario.OwnsOne(c => c.Perimetros);

        // Asignacion a contrato.
        cuestionario.HasOne(c => c.Contrato)
            .WithMany(a => a.Cuestionarios)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<Entrenador> Entrenadores { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Contrato> Asignaciones { get; set; }
    public DbSet<Cuestionario> Cuestionarios { get; set; }
}