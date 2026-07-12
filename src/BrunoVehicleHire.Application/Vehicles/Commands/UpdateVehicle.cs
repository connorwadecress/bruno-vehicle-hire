using BrunoVehicleHire.Application.Vehicles.Dtos;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.UpdateVehicle;

public sealed record UpdateVehicleCommand(
    Guid Id,
    string Make,
    string Model,
    int Year) : IRequest<VehicleDto?>;