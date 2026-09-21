using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BrunoVehicleHire.Api.Authentication;

public sealed class ApiKeyAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions> //framework inheritance
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    //this one is abstract on the base - no default behaviour so theres no base to call
    //compare OnModelCreating in the DbContext - thats virtual, has a default, so we call base there
    protected override Task<AuthenticateResult> HandleAuthenticateAsync() //abstract method on the base class - no default behaviour so theres no base to call
    {
        if (!Request.Headers.TryGetValue(
                ApiKeyAuthenticationDefaults.HeaderName,
                out StringValues headerValues)
            || headerValues.Count != 1
            || string.IsNullOrWhiteSpace(headerValues[0]))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "A single API key header is required."));
        }

        var expectedApiKey = _configuration["Security:ApiKey"];

        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            Logger.LogCritical(
                "API key authentication is enabled but " +
                "Security:ApiKey is not configured.");

            return Task.FromResult(
                AuthenticateResult.Fail(
                    "API key authentication is not configured."));
        }

        if (!KeysMatch(headerValues[0]!, expectedApiKey))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("The API key is invalid."));
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "api-key-client"),
            new Claim(
                ClaimTypes.Name,
                "API key client")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(
            principal,
            Scheme.Name);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }

    private static bool KeysMatch(
        string providedApiKey,
        string expectedApiKey)
    {
        var providedHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(providedApiKey));

        var expectedHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(expectedApiKey));

        return CryptographicOperations.FixedTimeEquals( //added this hardening as its an industrry standard prevent against timing attacks. 
            providedHash,
            expectedHash);
        //Constant-time comparison is correct practice for comparing any secret, and I'd keep it.
        //It just isn't what's protecting this application, and I wouldn't present it as though it were."
    }
}