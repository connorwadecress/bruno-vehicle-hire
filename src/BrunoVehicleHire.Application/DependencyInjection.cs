using BrunoVehicleHire.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BrunoVehicleHire.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(ApplicationAssemblyMarker).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(ApplicationAssemblyMarker).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        //singleton would mean one instance for the whole apps life -
        // if singleton depended on scoped service then DI one grab one instance first time and reuse it for every future reques

        return services;
    }
}