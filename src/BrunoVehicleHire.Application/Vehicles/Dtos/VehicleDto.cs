using BrunoVehicleHire.Domain.Entities;

namespace BrunoVehicleHire.Application.Vehicles.Dtos;

//outbound http contract
public sealed record VehicleDto( // record doesnt gurantee immutability 
                                 // if you switch to plain class then you lose free value equality ( ==) and deconstruction support
                                 // record is a reference type, so it can be nul
                                 
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