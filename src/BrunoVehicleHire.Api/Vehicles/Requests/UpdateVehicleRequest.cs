namespace BrunoVehicleHire.Api.Vehicles.Requests;

public sealed record UpdateVehicleRequest(
    string Make,
    string Model,
    int Year);