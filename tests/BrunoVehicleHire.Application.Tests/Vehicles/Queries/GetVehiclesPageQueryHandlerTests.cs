using BrunoVehicleHire.Application.Tests.TestDoubles;
using BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;
using BrunoVehicleHire.Domain.Entities;

namespace BrunoVehicleHire.Application.Tests.Vehicles.Queries;

public sealed class GetVehiclesPageQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithRequestedPage_ReturnsPagedDtosAndTotalCount()
    {
        var repository = new InMemoryVehicleRepository();
        var expectedVehicle = new Vehicle(
            "BB 222 GP",
            "Toyota",
            "Corolla",
            2024);

        await repository.AddAsync(
            new("CC 333 GP", "Ford", "Ranger", 2023),
            CancellationToken.None);
        await repository.AddAsync(
            new("AA 111 GP", "Honda", "Civic", 2022),
            CancellationToken.None);
        await repository.AddAsync(expectedVehicle, CancellationToken.None);

        var handler = new GetVehiclesPageQueryHandler(repository);
        var query = new GetVehiclesPageQuery(PageNumber: 2, PageSize: 1);

        var result = await handler.Handle(query, CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(expectedVehicle.Id, item.Id);
        Assert.Equal(expectedVehicle.RegistrationNumber, item.RegistrationNumber);
        Assert.Equal(expectedVehicle.Make, item.Make);
        Assert.Equal(expectedVehicle.Model, item.Model);
        Assert.Equal(expectedVehicle.Year, item.Year);
        Assert.Equal(expectedVehicle.CreatedDate, item.CreatedDate);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
    }
}
