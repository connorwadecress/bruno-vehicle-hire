using BrunoVehicleHire.Application.Vehicles.Dtos;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    string RegistrationNumber,
    string Make,
    string Model,
    int Year) : IRequest<VehicleDto>;