using BrunoVehicleHire.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BrunoVehicleHire.Api.OpenApi;

internal sealed class ApiKeySecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        var apiKeySchemeExists = authenticationSchemes.Any(
            scheme =>
                scheme.Name ==
                ApiKeyAuthenticationDefaults.Scheme);

        if (!apiKeySchemeExists)
        {
            return;
        }

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                [ApiKeyAuthenticationDefaults.Scheme] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Name =
                            ApiKeyAuthenticationDefaults.HeaderName,
                        Description =
                            "Set your local development API key here."
                    }
            };

        foreach (var pathItem in document.Paths.Values)
        {
            if (pathItem.Operations is null)
            {
                continue;
            }

            foreach (var operation in pathItem.Operations.Values)
            {
                operation.Security ??= [];

                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            ApiKeyAuthenticationDefaults.Scheme,
                            document)] = []
                    });
            }
        }
    }
}