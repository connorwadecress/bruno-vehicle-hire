using BrunoVehicleHire.Application.Common.Models;
using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Domain.Repositories;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;

public sealed class GetVehiclesPageQueryHandler
    : IRequestHandler<GetVehiclesPageQuery, PagedResult<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehiclesPageQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<PagedResult<VehicleDto>> Handle(
        GetVehiclesPageQuery request,
        CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleRepository.GetPageAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await _vehicleRepository.CountAsync(cancellationToken);

        var vehicleDtos = vehicles
            .Select(VehicleDto.FromEntity)
            .ToList();

        return new PagedResult<VehicleDto>(
            vehicleDtos,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}