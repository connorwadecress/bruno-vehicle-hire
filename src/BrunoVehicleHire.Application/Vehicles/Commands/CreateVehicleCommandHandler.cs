using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Domain.Entities;
using BrunoVehicleHire.Domain.Repositories;
using BrunoVehicleHire.Application.Common.Exceptions;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandHandler //sealed blocks inheritance
    : IRequestHandler<CreateVehicleCommand, VehicleDto>
    // this handler shouldnt be extended - its complete 
    // to extend this you will create a new handler 

    //anyway its missing virtual 
{
    private readonly IVehicleRepository _vehicleRepository;

    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleDto> Handle(
        CreateVehicleCommand request,
        CancellationToken cancellationToken)
    {
        //existence check
        var registrationNumberExists =
            await _vehicleRepository.RegistrationNumberExistsAsync(
                request.RegistrationNumber,
                cancellationToken);

        //duplicate exception
        if (registrationNumberExists)
        {
            throw new DuplicateVehicleRegistrationException();
        }

        //construct the vehicle
        var vehicle = new Vehicle(
            request.RegistrationNumber,
            request.Make,
            request.Model,
            request.Year);

        // add and save c hanges are split because of unit of work pattern - you can add multiple entities and then save changes once
        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return VehicleDto.FromEntity(vehicle);
    }
}