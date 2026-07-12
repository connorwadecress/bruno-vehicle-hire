using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.DeleteVehicle;

public sealed record DeleteVehicleCommand(Guid Id) : IRequest<bool>;