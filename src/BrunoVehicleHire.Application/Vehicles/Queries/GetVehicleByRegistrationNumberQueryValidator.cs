using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehicleByRegistrationNumber;

public sealed class GetVehicleByRegistrationNumberQueryValidator
    : AbstractValidator<GetVehicleByRegistrationNumberQuery>
{
    public GetVehicleByRegistrationNumberQueryValidator()
    {
        RuleFor(query => query.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}