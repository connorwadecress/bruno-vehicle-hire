using BrunoVehicleHire.Application.Common.Exceptions;
using BrunoVehicleHire.Domain.Entities;
using BrunoVehicleHire.Infrastructure.Persistence;
using BrunoVehicleHire.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BrunoVehicleHire.Infrastructure.Tests.Repositories;

// These tests run against a real SQLite database held in memory, so the unique index and the
// soft-delete query filter are actually exercised rather than simulated by a test double.
public sealed class VehicleRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly BrunoVehicleHireDbContext _dbContext;
    private readonly VehicleRepository _repository;

    public VehicleRepositoryTests()
    {
        // The in-memory database only lives as long as the connection is open.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<BrunoVehicleHireDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new BrunoVehicleHireDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new VehicleRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task SaveChangesAsync_WhenRegistrationNumberIsAlreadyUsed_ThrowsDuplicateRegistration()
    {
        await AddAndSaveAsync(new Vehicle("CA 123 GP", "Toyota", "Corolla", 2024));

        // Bypasses the handler's existence check, which is what a concurrent request effectively does.
        await _repository.AddAsync(
            new Vehicle("CA 123 GP", "Honda", "Civic", 2025),
            CancellationToken.None);

        await Assert.ThrowsAsync<DuplicateVehicleRegistrationException>(
            () => _repository.SaveChangesAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetPageAsync_ExcludesSoftDeletedVehicles()
    {
        var deletedVehicle = new Vehicle("BB 222 GP", "Honda", "Civic", 2022);
        deletedVehicle.SoftDelete();

        await AddAndSaveAsync(
            new Vehicle("AA 111 GP", "Ford", "Ranger", 2023),
            deletedVehicle);

        var page = await _repository.GetPageAsync(1, 10, CancellationToken.None);

        var remainingVehicle = Assert.Single(page);
        Assert.Equal("AA 111 GP", remainingVehicle.RegistrationNumber);
        Assert.Equal(1, await _repository.CountAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetByRegistrationNumberAsync_WhenVehicleIsSoftDeleted_ReturnsNull()
    {
        var deletedVehicle = new Vehicle("BB 222 GP", "Honda", "Civic", 2022);
        deletedVehicle.SoftDelete();

        await AddAndSaveAsync(deletedVehicle);

        var found = await _repository.GetByRegistrationNumberAsync(
            "BB 222 GP",
            CancellationToken.None);

        Assert.Null(found);
    }

    [Fact]
    public async Task RegistrationNumberExistsAsync_WhenVehicleIsSoftDeleted_ReturnsTrue()
    {
        // Deliberate: the unique index covers deleted rows too, so reusing a retired plate has to
        // fail the check rather than passing it and then breaking on insert.
        var deletedVehicle = new Vehicle("BB 222 GP", "Honda", "Civic", 2022);
        deletedVehicle.SoftDelete();

        await AddAndSaveAsync(deletedVehicle);

        var exists = await _repository.RegistrationNumberExistsAsync(
            "bb 222 gp",
            CancellationToken.None);

        Assert.True(exists);
    }

    private async Task AddAndSaveAsync(params Vehicle[] vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            await _repository.AddAsync(vehicle, CancellationToken.None);
        }

        await _repository.SaveChangesAsync(CancellationToken.None);
    }
}
