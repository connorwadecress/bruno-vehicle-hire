using BrunoVehicleHire.Application.Tests.TestDoubles;
using BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

namespace BrunoVehicleHire.Application.Tests.Vehicles.Commands;

public sealed class CreateVehicleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNewRegistration_AddsNormalizedVehicleAndSavesChanges()
    {
        var repository = new InMemoryVehicleRepository();
        var handler = new CreateVehicleCommandHandler(repository);
        var command = new CreateVehicleCommand(
            " ca 123 gp ",
            " Toyota ",
            " Corolla ",
            2024);

        var result = await handler.Handle(command, CancellationToken.None);

        var storedVehicle = Assert.Single(repository.Vehicles);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(storedVehicle.Id, result.Id);
        Assert.Equal("CA 123 GP", result.RegistrationNumber);
        Assert.Equal("Toyota", result.Make);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(2024, result.Year);
        Assert.Equal(storedVehicle.CreatedDate, result.CreatedDate);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithDuplicateRegistration_ThrowsWithoutAddingOrSaving()
    {
        var repository = new InMemoryVehicleRepository();
        await repository.AddAsync(
            new("CA 123 GP", "Toyota", "Corolla", 2024),
            CancellationToken.None);

        var handler = new CreateVehicleCommandHandler(repository);
        var command = new CreateVehicleCommand(
            " ca 123 gp ",
            "Honda",
            "Civic",
            2025);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(
            "A vehicle with this registration number already exists.",
            exception.Message);
        Assert.Single(repository.Vehicles);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }
}
