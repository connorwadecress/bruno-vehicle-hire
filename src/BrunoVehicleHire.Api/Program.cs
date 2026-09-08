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
    .AddAuthentication(options => //has to run before autheorization
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

builder.Services.AddAuthorizationBuilder() // calls the ApiKeyAuthenticationHandler to check the API key and authenticate the user
    // autheorization needs to know who the caller is before it can decide whether to let them through

    // doesnt do credential checking - just looks at whatver identity sits on 
    .SetFallbackPolicy(
        new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(
                ApiKeyAuthenticationDefaults.Scheme)
            .RequireAuthenticatedUser() //What are you allowed to do? 
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

app.UseExceptionHandler(); //needs to wrap everything - if any exception is thrown, the handler cartches it and returns a problem details response
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
