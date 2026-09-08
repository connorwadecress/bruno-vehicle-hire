using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {

        //only checks registration number is empty or max length - nothing to do with duplication

        //fluent validation does support async rules however: validators here are to check shape not state
        // the duplocate check needs to hit the reposutory and decide what exception to throw, so that is done in the handler
        RuleFor(command => command.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(20);

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
