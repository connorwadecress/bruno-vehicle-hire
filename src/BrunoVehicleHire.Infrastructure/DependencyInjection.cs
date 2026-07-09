using BrunoVehicleHire.Domain.Repositories;
using BrunoVehicleHire.Infrastructure.Persistence;
using BrunoVehicleHire.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BrunoVehicleHire.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BrunoVehicleHireDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IVehicleRepository, VehicleRepository>();

        return services;
    }
}