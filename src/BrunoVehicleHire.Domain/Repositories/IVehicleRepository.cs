using BrunoVehicleHire.Domain.Entities;

namespace BrunoVehicleHire.Domain.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken);
    Task<IReadOnlyList<Vehicle>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<bool> RegistrationNumberExistsAsync(string registrationNumber, CancellationToken cancellationToken);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}