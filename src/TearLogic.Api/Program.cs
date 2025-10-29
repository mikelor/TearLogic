using System.Net.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Service.CBInsights.Configuration;
using Service.CBInsights.Endpoints;
using Service.CBInsights.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<CBInsightsOptions>()
    .Bind(builder.Configuration.GetSection(CBInsightsOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(options => !string.IsNullOrWhiteSpace(options.BaseUrl), "BaseUrl must be provided.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "ClientId must be provided.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret), "ClientSecret must be provided.")
    .ValidateOnStart();

builder.Services.AddHttpClient(ICBInsightsClientFactory.HttpClientName, static (serviceProvider, client) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<CBInsightsOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl!, UriKind.Absolute);
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddSingleton<ICBInsightsClientFactory, CBInsightsClientFactory>();
builder.Services.AddSingleton<ICBInsightsTokenProvider, CBInsightsTokenProvider>();
builder.Services.AddSingleton<ICBInsightsOrganizationService, CBInsightsOrganizationService>();
builder.Services.AddSingleton<OrgLookupCommandHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOrgLookupEndpoint();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 250)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30));
