namespace BrunoVehicleHire.Api.Vehicles.Requests;

//inbound http contract
public sealed record CreateVehicleRequest(
    string RegistrationNumber,
    string Make,
    string Model,
    int Year);