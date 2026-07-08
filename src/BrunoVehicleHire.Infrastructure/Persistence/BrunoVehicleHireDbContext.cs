using BrunoVehicleHire.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrunoVehicleHire.Infrastructure.Persistence;

public class BrunoVehicleHireDbContext : DbContext
{
    public BrunoVehicleHireDbContext(DbContextOptions<BrunoVehicleHireDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicles");

            entity.HasKey(vehicle => vehicle.Id);

            entity.Property(vehicle => vehicle.RegistrationNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(vehicle => vehicle.RegistrationNumber)
                .IsUnique();

            entity.Property(vehicle => vehicle.Make)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(vehicle => vehicle.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(vehicle => vehicle.Year)
                .IsRequired();

            entity.Property(vehicle => vehicle.CreatedDate)
                .IsRequired();

            entity.Property(vehicle => vehicle.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasQueryFilter(vehicle => !vehicle.IsDeleted);
        });
    }
}