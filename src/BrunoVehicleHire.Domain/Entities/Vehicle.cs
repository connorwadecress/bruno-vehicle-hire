namespace BrunoVehicleHire.Domain.Entities;

public class Vehicle
{
    // materialization - turning database row into an object
    private Vehicle()
    {
        //empty shell to for already saved column values into the properies (bypass business logic)

        //exists so EF can use a row without re running the business logic
        // private setters prevent anything outside thos class from changing the properties 
    }

    public Vehicle(string registrationNumber, string make, string model, int year)
    {
        Id = Guid.NewGuid(); 
        RegistrationNumber = registrationNumber.Trim().ToUpperInvariant();
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        CreatedDate = DateTime.UtcNow; // this is why we have paramereterless constructor
        //EF needs to be able to create an instance of the entity without calling the constructor with parameters
        IsDeleted = false;
    }

    //all mutations have to go through the constructor

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
