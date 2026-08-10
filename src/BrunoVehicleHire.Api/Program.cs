using BrunoVehicleHire.Api.Authentication;
using BrunoVehicleHire.Api.Exceptions;
using BrunoVehicleHire.Api.OpenApi;
using BrunoVehicleHire.Application;
using BrunoVehicleHire.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var allowedFrontendOrigin = builder.Configuration["Cors:AllowedOrigin"]
    ?? throw new InvalidOperationException(
        "CORS allowed frontend origin was not configured.");

const string frontendCorsPolicy = "Frontend";

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedFrontendOrigin)
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders(
                "Content-Type",
                ApiKeyAuthenticationDefaults.HeaderName);
    });
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            ApiKeyAuthenticationDefaults.Scheme;

        options.DefaultChallengeScheme =
            ApiKeyAuthenticationDefaults.Scheme;
    })
    .AddScheme<
        AuthenticationSchemeOptions,
        ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationDefaults.Scheme,
            displayName: null,
            configureOptions: _ => { });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(
        new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(
                ApiKeyAuthenticationDefaults.Scheme)
            .RequireAuthenticatedUser()
            .Build());

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        ApiKeySecuritySchemeTransformer>();
});

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(frontendCorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Bruno Vehicle Hire API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
