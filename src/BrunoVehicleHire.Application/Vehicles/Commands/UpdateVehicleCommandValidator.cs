using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleCommandValidator
    : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

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