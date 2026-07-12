using BrunoVehicleHire.Application.Common.Models;
using BrunoVehicleHire.Application.Vehicles.Dtos;
using MediatR;

namespace BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;

public sealed record GetVehiclesPageQuery(
    int PageNumber,
    int PageSize) : IRequest<PagedResult<VehicleDto>>;