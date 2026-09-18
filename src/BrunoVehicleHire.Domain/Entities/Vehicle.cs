namespace BrunoVehicleHire.Domain.Entities;

public class Vehicle
{
    // materialization - turning database row into an object
    private Vehicle()
    {
        //empty shell to for already saved column values into the properies (bypass business logic)

        //exists so EF can use a row without re running the business logic
        // private setters prevent anything outside thos class from changing the properties 

        // NOT SEALED: unsealed by default and no specific reason to lock it down
    }

    public Vehicle(string registrationNumber, string make, string model, int year)  //builder pattern vibes
      //4 params, easy to swap by mistake (3 are strings) - use named args at call sites
       //would move to a builder/parameter object if this grows past ~6 params, not worth it yet
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
