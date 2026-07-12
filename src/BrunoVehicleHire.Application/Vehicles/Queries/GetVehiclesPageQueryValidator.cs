using FluentValidation;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;

public sealed class GetVehiclesPageQueryValidator
    : AbstractValidator<GetVehiclesPageQuery>
{
    public GetVehiclesPageQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);
    }
}