using BrunoVehicleHire.Domain.Entities;
using BrunoVehicleHire.Domain.Repositories;
using BrunoVehicleHire.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BrunoVehicleHire.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly BrunoVehicleHireDbContext _dbContext;

    public VehicleRepository(BrunoVehicleHireDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Vehicles
            .FirstOrDefaultAsync(vehicle => vehicle.Id == id, cancellationToken);
    }

    public Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken)
    {
        var normalizedRegistrationNumber = registrationNumber.Trim().ToUpperInvariant();

        return _dbContext.Vehicles
            .FirstOrDefaultAsync(
                vehicle => vehicle.RegistrationNumber == normalizedRegistrationNumber,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Vehicle>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Vehicles
            .OrderBy(vehicle => vehicle.RegistrationNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Vehicles.CountAsync(cancellationToken);
    }

    public Task<bool> RegistrationNumberExistsAsync(string registrationNumber, CancellationToken cancellationToken)
    {
        var normalizedRegistrationNumber = registrationNumber.Trim().ToUpperInvariant();

        return _dbContext.Vehicles
            .IgnoreQueryFilters()
            .AnyAsync(vehicle => vehicle.RegistrationNumber == normalizedRegistrationNumber, cancellationToken);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken)
    {
        await _dbContext.Vehicles.AddAsync(vehicle, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
