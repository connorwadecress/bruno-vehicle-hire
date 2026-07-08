namespace BrunoVehicleHire.Domain.Entities;

public class Vehicle
{
    private Vehicle()
    {
    }

    public Vehicle(string registrationNumber, string make, string model, int year)
    {
        Id = Guid.NewGuid();
        RegistrationNumber = registrationNumber.Trim().ToUpperInvariant();
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        CreatedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public Guid Id { get; private set; }
    public string RegistrationNumber { get; private set; } = string.Empty;
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedDate { get; private set; }

    public void Update(string make, string model, int year)
    {
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
