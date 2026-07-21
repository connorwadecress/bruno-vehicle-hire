namespace BrunoVehicleHire.Application.Common.Exceptions;

public sealed class DuplicateVehicleRegistrationException : Exception
{
    public DuplicateVehicleRegistrationException()
        : base("A vehicle with this registration number already exists.")
    {
    }
}