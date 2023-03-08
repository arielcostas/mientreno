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
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
    }

    public DbSet<Usuario> Usuarios { get; set; }
}