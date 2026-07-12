using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Commands.DeleteVehicle;

public sealed class DeleteVehicleCommandValidator
    : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}