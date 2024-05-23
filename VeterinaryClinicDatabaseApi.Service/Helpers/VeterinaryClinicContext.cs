using Microsoft.EntityFrameworkCore;
using VeterinaryClinicDatabaseApi.Service.Models;

namespace VeterinaryClinicDatabaseApi.Service.Helpers;

public class VeterinaryClinicContext : DbContext
{
    public VeterinaryClinicContext(DbContextOptions<VeterinaryClinicContext> options) : base(options)
    {
        Console.WriteLine(options);
    }
    
    public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToTable("Animal");
            entity.HasKey(e => e.IdAnimal);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Area).IsRequired().HasMaxLength(200);
        });
    }
}