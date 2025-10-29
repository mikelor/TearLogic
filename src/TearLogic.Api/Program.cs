using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http.Headers;
using TearLogic.Api.Options;
using TearLogic.Api.Services.Abstractions;
using TearLogic.Api.Services.Internal;
using TearLogic.Api.Validation;
using TearLogic.Clients;
using TearLogic.Clients.Models.V2OrganizationLookup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<CBInsightsOptions>()
    .Bind(builder.Configuration.GetSection(CBInsightsOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(options => !string.IsNullOrWhiteSpace(options.BaseUrl), "CB Insights base URL must be provided.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "CB Insights client id must be provided.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "CB Insights client secret must be provided.")
    .Validate(options => options.TokenCacheDuration > TimeSpan.Zero, "Token cache duration must be greater than zero.")
    .ValidateOnStart();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddSingleton<ICBInsightsTokenProvider, CBInsightsTokenProvider>();
builder.Services.AddSingleton<IAuthenticationProvider, CBInsightsAuthenticationProvider>();
builder.Services.AddScoped<ICBInsightsOrganizationLookupService, CBInsightsOrganizationLookupService>();

builder.Services.AddHttpClient(HttpClientNames.Authorization, ConfigureAuthorizationClient)
    .AddPolicyHandler(CreateRetryPolicy());

builder.Services.AddHttpClient(HttpClientNames.Api, ConfigureApiClient)
    .AddPolicyHandler(CreateRetryPolicy())
    .AddPolicyHandler(CreateCircuitBreakerPolicy());

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/cbi/org-lookup", async (
    [FromBody] OrganizationLookupRequest request,
    ICBInsightsOrganizationLookupService service,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("CBInsightsOrgLookupEndpoint");

    if (!request.HasLookupCriteria)
    {
        return Results.BadRequest(new
        {
            error = "At least one lookup parameter (names, urls, profileUrl, or nextPageToken) must be provided."
        });
    }

    try
    {
        var response = await service.LookupOrganizationsAsync(request, cancellationToken).ConfigureAwait(false);
        return Results.Json(response ?? new OrgLookupResponse());
    }
    catch (Microsoft.Kiota.Abstractions.ApiException ex)
    {
        logger.LogError(ex, "CB Insights returned an error with status code {StatusCode}.", ex.ResponseStatusCode);
        var statusCode = ex.ResponseStatusCode is int code ? code : StatusCodes.Status502BadGateway;
        return Results.Problem(title: "CB Insights request failed.", detail: ex.Message, statusCode: statusCode);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error while calling CB Insights organization lookup.");
        return Results.Problem(title: "Unexpected error.", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).WithName("OrganizationLookup");

app.Run();

static void ConfigureAuthorizationClient(IServiceProvider serviceProvider, HttpClient client)
{
    var options = serviceProvider.GetRequiredService<IOptions<CBInsightsOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.UserAgent.ParseAdd("TearLogic.Api/1.0");
}

static void ConfigureApiClient(IServiceProvider serviceProvider, HttpClient client)
{
    var options = serviceProvider.GetRequiredService<IOptions<CBInsightsOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.UserAgent.ParseAdd("TearLogic.Api/1.0");
}

static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt =>
            TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 250)));
}

static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

namespace TearLogic.Api
{
    internal static class HttpClientNames
    {
        public const string Authorization = "CBInsightsAuthorization";
        public const string Api = "CBInsightsApi";
    }
}
