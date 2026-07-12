using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Domain.Repositories;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleCommandHandler
    : IRequestHandler<UpdateVehicleCommand, VehicleDto?>
{
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleDto?> Handle(
        UpdateVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (vehicle is null)
        {
            return null;
        }

        vehicle.Update(
            request.Make,
            request.Model,
            request.Year);

        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return VehicleDto.FromEntity(vehicle);
    }
}