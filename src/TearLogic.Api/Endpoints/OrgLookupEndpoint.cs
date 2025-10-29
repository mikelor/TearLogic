using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using Service.CBInsights.Commands;
using Service.CBInsights.Models;
using Service.CBInsights.Services;

namespace Service.CBInsights.Endpoints;

/// <summary>
/// Provides the endpoint definition for organization lookup operations.
/// </summary>
public static class OrgLookupEndpoint
{
    private static readonly ResourceManager ErrorResourceManager = new("Service.CBInsights.Resources.ErrorMessages", typeof(OrgLookupEndpoint).Assembly);

    /// <summary>
    /// Maps the organization lookup endpoint to the provided route builder.
    /// </summary>
    /// <param name="endpoints">The application endpoint route builder.</param>
    /// <returns>The supplied route builder.</returns>
    public static IEndpointRouteBuilder MapOrgLookupEndpoint(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost("/cbi/org-lookup", async Task<IResult> (OrgLookupRequest request, OrgLookupCommandHandler handler, CancellationToken cancellationToken) =>
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(handler);

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, validateAllProperties: true))
            {
                var errors = validationResults
                    .SelectMany(result =>
                        result.MemberNames.Any()
                            ? result.MemberNames.Select(member => new KeyValuePair<string, string>(member, result.ErrorMessage ?? string.Empty))
                            : new[] { new KeyValuePair<string, string>(string.Empty, result.ErrorMessage ?? string.Empty) })
                    .GroupBy(pair => string.IsNullOrWhiteSpace(pair.Key) ? "request" : pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => group.Where(message => !string.IsNullOrWhiteSpace(message)).Distinct().ToArray());

                return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest, title: ErrorResourceManager.GetString("RequestValidationFailed"));
            }

            var response = await handler.HandleAsync(new OrgLookupCommand(request), cancellationToken).ConfigureAwait(false);
            return Results.Ok(response);
        })
        .WithName("OrgLookup")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Executes a CB Insights organization lookup.";
            operation.Tags = new[] { "CB Insights" };
            return operation;
        });

        return endpoints;
    }
}
