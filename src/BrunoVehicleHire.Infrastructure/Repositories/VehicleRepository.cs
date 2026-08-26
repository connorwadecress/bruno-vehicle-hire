using BrunoVehicleHire.Application.Common.Exceptions;
using BrunoVehicleHire.Domain.Entities;
using BrunoVehicleHire.Domain.Repositories;
using BrunoVehicleHire.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BrunoVehicleHire.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private const int SqliteConstraintViolation = 19;

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsRegistrationNumberConflict(exception))
        {
            throw new DuplicateVehicleRegistrationException();
        }
    }

    // The handler's existence check gives a friendly 409 on the normal path, but two concurrent
    // creates can both pass it. The unique index is what actually prevents the duplicate row, and
    // SQLite reports that as "UNIQUE constraint failed: Vehicles.RegistrationNumber". Only that
    // specific failure becomes a duplicate-registration conflict - anything else keeps bubbling.
    private static bool IsRegistrationNumberConflict(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqliteException
            && sqliteException.SqliteErrorCode == SqliteConstraintViolation
            && sqliteException.Message.Contains(
                "Vehicles.RegistrationNumber",
                StringComparison.OrdinalIgnoreCase);
    }
}
