using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand> //framework inheritance
    //same deal as the DbContext - fluent validation designed this slot, we just fill it
{
    public CreateVehicleCommandValidator()
    {

        //only checks registration number is empty or max length - nothing to do with duplication

        //fluent validation does support async rules however: validators here are to check shape not state
        // the duplocate check needs to hit the reposutory and decide what exception to throw, so that is done in the handler
        RuleFor(command => command.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(20); // we have 2 places we check this. here and in the DbContext 
        // fleunt validation only runs if a request goes through MediatR pipeline
        //DbContext constraint protects against every writer (second app / raw SQL script, bad migration, db access etc) - its a unique index on the column in the database

        // if this business logic changes we have to update in 2 places

        RuleFor(command => command.Make)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Model)
            .NotEmpty()
            .MaximumLength(100);

        //same for year - just checks it's a reasonable year, not duplication

        RuleFor(command => command.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1);
    }
}
