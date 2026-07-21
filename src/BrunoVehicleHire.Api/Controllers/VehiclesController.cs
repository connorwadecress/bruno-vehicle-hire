using BrunoVehicleHire.Application.Common.Models;
using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using BrunoVehicleHire.Application.Vehicles.Queries.GetVehicleByRegistrationNumber;

namespace BrunoVehicleHire.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
[Produces("application/json")]
public sealed class VehiclesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResult<VehicleDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<VehicleDto>>> GetPage(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVehiclesPageQuery(
            pageNumber,
            pageSize);

        var result = await sender.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("registration/{registrationNumber}")]
    [ProducesResponseType(
    typeof(VehicleDto),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(
    typeof(ValidationProblemDetails),
    StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> GetByRegistrationNumber(
    string registrationNumber,
    CancellationToken cancellationToken)
    {
        var query = new GetVehicleByRegistrationNumberQuery(
            registrationNumber);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result);
    }
}