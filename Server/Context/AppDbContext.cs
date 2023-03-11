using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Tabla común de usuarios con dos tipos de usuarios: Alumnos y Entrenadores.
        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("Tipo")
            .HasValue<Entrenador>("Entrenador")
            .HasValue<Alumno>("Alumno");

        // Credenciales de usuario.
        modelBuilder.Entity<Usuario>()
            .OwnsOne(u => u.Credenciales);
        
        // Cuestionario tiene hábitos y perímetros.
        modelBuilder.Entity<Cuestionario>()
            .OwnsOne(c => c.Habitos);
        
        modelBuilder.Entity<Cuestionario>()
            .OwnsOne(c => c.Perimetros);
        
        // Asignacion a entrenador.
        modelBuilder.Entity<Contrato>()
            .HasOne(a => a.Entrenador)
            .WithMany(e => e.Asignaciones)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        
        // Asignacion a alumno.
        modelBuilder.Entity<Contrato>()
            .HasOne(a => a.Alumno)
            .WithMany(a => a.Asignaciones)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        
        // Asignacion a contrato.
        modelBuilder.Entity<Cuestionario>()
            .HasOne(c => c.Contrato)
            .WithMany(a => a.Cuestionarios)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Entrenador> Entrenadores { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Contrato> Asignaciones { get; set; }
    public DbSet<Cuestionario> Cuestionarios { get; set; }
}