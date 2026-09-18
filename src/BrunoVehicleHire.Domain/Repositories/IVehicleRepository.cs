using BrunoVehicleHire.Domain.Entities;

namespace BrunoVehicleHire.Domain.Repositories;

public interface IVehicleRepository // consumers depend on this - consumers dont care which implementation they get (infrastructure or in memory)
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken);
    Task<IReadOnlyList<Vehicle>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<bool> RegistrationNumberExistsAsync(string registrationNumber, CancellationToken cancellationToken);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

// repository pattern + liskov substitution principle - consumers depend on the interface, not the implementation

// we use repository pattern for testability 
// easy to swap out the implementation for a mock or in-memory version for testing

// bruno requrement is : "Application layer must not depend on infrastructure,"

// now the soft delete bypass stays centralised in the repository implementation, so that the application layer does not need to know about it.