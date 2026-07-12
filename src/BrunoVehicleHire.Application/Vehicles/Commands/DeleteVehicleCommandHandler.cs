using BrunoVehicleHire.Domain.Repositories;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.DeleteVehicle;

public sealed class DeleteVehicleCommandHandler
    : IRequestHandler<DeleteVehicleCommand, bool>
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<bool> Handle(
        DeleteVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (vehicle is null)
        {
            return false;
        }

        vehicle.SoftDelete();

        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}