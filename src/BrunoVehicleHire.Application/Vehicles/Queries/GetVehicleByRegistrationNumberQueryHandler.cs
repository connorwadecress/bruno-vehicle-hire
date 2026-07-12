using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Domain.Repositories;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehicleByRegistrationNumber;

public sealed class GetVehicleByRegistrationNumberQueryHandler
    : IRequestHandler<GetVehicleByRegistrationNumberQuery, VehicleDto?>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleByRegistrationNumberQueryHandler(
        IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleDto?> Handle(
        GetVehicleByRegistrationNumberQuery request,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByRegistrationNumberAsync(
            request.RegistrationNumber,
            cancellationToken);

        return vehicle is null
            ? null
            : VehicleDto.FromEntity(vehicle);
    }
}