using BrunoVehicleHire.Domain.Entities;

namespace BrunoVehicleHire.Application.Vehicles.Dtos;

public sealed record VehicleDto(
    Guid Id,
    string RegistrationNumber,
    string Make,
    string Model,
    int Year,
    DateTime CreatedDate)
{
    public static VehicleDto FromEntity(Vehicle vehicle)
    {
        return new VehicleDto(
            vehicle.Id,
            vehicle.RegistrationNumber,
            vehicle.Make,
            vehicle.Model,
            vehicle.Year,
            vehicle.CreatedDate);
    }
}