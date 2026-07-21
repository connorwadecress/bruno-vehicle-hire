using BrunoVehicleHire.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BrunoVehicleHire.Api.Exceptions;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = exception switch
        {
            ValidationException validationException =>
                CreateValidationProblemDetails(
                    httpContext,
                    validationException),

            DuplicateVehicleRegistrationException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Registration number already exists.",
                Detail =
                    "A vehicle with this registration number already exists.",
                Instance = httpContext.Request.Path
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Instance = httpContext.Request.Path
            }
        };

        if (exception is ValidationException
            or DuplicateVehicleRegistrationException)
        {
            logger.LogWarning(
                exception,
                "Request failed for {Path}.",
                httpContext.Request.Path);
        }
        else
        {
            logger.LogError(
                exception,
                "Unhandled exception for {Path}.",
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };
    }
}