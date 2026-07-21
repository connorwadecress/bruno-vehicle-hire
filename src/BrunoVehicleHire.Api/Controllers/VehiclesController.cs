using BrunoVehicleHire.Application.Common.Models;
using BrunoVehicleHire.Application.Vehicles.Dtos;
using BrunoVehicleHire.Application.Vehicles.Queries.GetVehiclesPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BrunoVehicleHire.Application.Vehicles.Queries.GetVehicleByRegistrationNumber;
using BrunoVehicleHire.Api.Vehicles.Requests;
using BrunoVehicleHire.Application.Vehicles.Commands.CreateVehicle;

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

    [HttpPost]
    [ProducesResponseType(
        typeof(VehicleDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VehicleDto>> Create(
        [FromBody] CreateVehicleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVehicleCommand(
            request.RegistrationNumber,
            request.Make,
            request.Model,
            request.Year);

        var result = await sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByRegistrationNumber),
            new
            {
                registrationNumber = result.RegistrationNumber
            },
            result);
    }
}