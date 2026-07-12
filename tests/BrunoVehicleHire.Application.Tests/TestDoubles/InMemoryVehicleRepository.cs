using BrunoVehicleHire.Domain.Entities;
using BrunoVehicleHire.Domain.Repositories;

namespace BrunoVehicleHire.Application.Tests.TestDoubles;

// Test double: this replaces the EF Core repository only in unit tests.
public sealed class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles = [];

    public IReadOnlyList<Vehicle> Vehicles => _vehicles;

    public int SaveChangesCallCount { get; private set; }

    public Task<Vehicle?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicles.SingleOrDefault(vehicle =>
            vehicle.Id == id && !vehicle.IsDeleted);

        return Task.FromResult(vehicle);
    }

    public Task<Vehicle?> GetByRegistrationNumberAsync(
        string registrationNumber,
        CancellationToken cancellationToken)
    {
        var normalizedRegistrationNumber =
            registrationNumber.Trim().ToUpperInvariant();

        var vehicle = _vehicles.SingleOrDefault(vehicle =>
            vehicle.RegistrationNumber == normalizedRegistrationNumber
            && !vehicle.IsDeleted);

        return Task.FromResult(vehicle);
    }

    public Task<IReadOnlyList<Vehicle>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Vehicle> vehicles = _vehicles
            .Where(vehicle => !vehicle.IsDeleted)
            .OrderBy(vehicle => vehicle.RegistrationNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(vehicles);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        var count = _vehicles.Count(vehicle => !vehicle.IsDeleted);

        return Task.FromResult(count);
    }

    public Task<bool> RegistrationNumberExistsAsync(
        string registrationNumber,
        CancellationToken cancellationToken)
    {
        var normalizedRegistrationNumber =
            registrationNumber.Trim().ToUpperInvariant();

        var exists = _vehicles.Any(vehicle =>
            vehicle.RegistrationNumber == normalizedRegistrationNumber);

        return Task.FromResult(exists);
    }

    public Task AddAsync(
        Vehicle vehicle,
        CancellationToken cancellationToken)
    {
        _vehicles.Add(vehicle);

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;

        return Task.CompletedTask;
    }
}