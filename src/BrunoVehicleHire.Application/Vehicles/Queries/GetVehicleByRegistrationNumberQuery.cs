using BrunoVehicleHire.Application.Vehicles.Dtos;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehicleByRegistrationNumber;

public sealed record GetVehicleByRegistrationNumberQuery(
    string RegistrationNumber) : IRequest<VehicleDto?>;