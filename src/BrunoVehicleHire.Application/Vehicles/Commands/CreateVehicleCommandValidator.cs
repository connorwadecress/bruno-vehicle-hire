using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(command => command.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(command => command.Make)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Model)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1);
    }
}
