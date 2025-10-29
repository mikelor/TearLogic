using System.Net;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Extensions.Http;
using TearLogic.Api;
using TearLogic.Api.CBInsights;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<CBInsightsOptions>(builder.Configuration.GetSection(CBInsightsOptions.SectionName));
builder.Services.AddOptions<CBInsightsOptions>()
    .ValidateDataAnnotations()
    .Validate(static options => !string.Equals(options.ClientId, "YOUR_CLIENT_ID", StringComparison.OrdinalIgnoreCase), "CB Insights client ID must be configured.")
    .Validate(static options => !string.Equals(options.ClientSecret, "YOUR_CLIENT_SECRET", StringComparison.OrdinalIgnoreCase), "CB Insights client secret must be configured.")
    .ValidateOnStart();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient(CBInsightsHttpClientNames.Api, client =>
    {
        client.BaseAddress = new Uri("https://api.cbinsights.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient(CBInsightsHttpClientNames.Auth, client =>
    {
        client.BaseAddress = new Uri("https://api.cbinsights.com");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddSingleton<ICBInsightsTokenService, CBInsightsTokenService>();
builder.Services.AddSingleton<CBInsightsAuthenticationProvider>();
builder.Services.AddSingleton<ICBInsightsClientFactory, CBInsightsClientFactory>();
builder.Services.AddScoped<IOrganizationLookupService, OrganizationLookupService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

app.MapPost("/cbi/org-lookup", async Task<IResult> (
        [FromBody] OrganizationLookupRequest request,
        IOrganizationLookupService service,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken) =>
    {
        var logger = loggerFactory.CreateLogger("OrgLookupEndpoint");
        if (!request.HasSearchCriteria)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = "At least one search parameter (names, urls, or profileUrl) must be provided.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var response = await service.LookupAsync(request, cancellationToken).ConfigureAwait(false);
            return Results.Ok(response);
        }
        catch (CBInsightsAuthenticationException ex)
        {
            logger.LogError(ex, "CB Insights authentication failed.");
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status502BadGateway, title: "CB Insights authentication error");
        }
        catch (CBInsightsRequestException ex)
        {
            logger.LogError(ex, "CB Insights request failed with status code {StatusCode}", (int)ex.StatusCode);
            return Results.Problem(detail: ex.Message, statusCode: (int)ex.StatusCode, title: "CB Insights request error");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error while calling CB Insights.");
            return Results.Problem(detail: "Unable to reach CB Insights at this time.", statusCode: StatusCodes.Status503ServiceUnavailable, title: "Service unavailable");
        }
    })
    .WithName("OrganizationLookup")
    .Produces<TearLogic.Clients.Models.V2OrganizationLookup.OrgLookupResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status502BadGateway)
    .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(static response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3,
            static attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(static response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
