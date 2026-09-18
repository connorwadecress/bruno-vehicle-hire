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
                .IsRequired() //generates real not null column in database
                // if removed the compiler would not catch
                .HasMaxLength(20);

            entity.HasIndex(vehicle => vehicle.RegistrationNumber)
                .IsUnique(); //performance index -> to make lookips and sort by registration number  faster
            // if we remove it then RegistrationNumberExistsAsync will be slower because it will have to scan the entire table to check for existence

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

            // global query filter applies to every LINQ query
            entity.HasQueryFilter(vehicle => !vehicle.IsDeleted);
        });
    }
}