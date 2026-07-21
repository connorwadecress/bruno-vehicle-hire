namespace BrunoVehicleHire.Api.Vehicles.Requests;

public sealed record CreateVehicleRequest(
    string RegistrationNumber,
    string Make,
    string Model,
    int Year);