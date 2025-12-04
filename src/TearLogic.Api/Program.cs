using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using TearLogic.Api.CBInsights.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddApplicationPart(typeof(OrganizationLookupController).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(static options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "TearLogic CB Insights API",
            Version = "v1",
            Description = "API endpoints that proxy CB Insights organization lookup capabilities for TearLogic.",
            Contact = new OpenApiContact
            {
                Name = "TearLogic",
                Url = new Uri("https://www.tearlogic.com", UriKind.Absolute)
            }
        });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlFilePath))
    {
        options.IncludeXmlComments(xmlFilePath);
    }

    options.SupportNonNullableReferenceTypes();
    options.CustomSchemaIds(static type =>
    {
        if (type.FullName is null)
        {
            return type.Name;
        }

        return type.FullName
            .Replace("TearLogic.", string.Empty, StringComparison.Ordinal)
            .Replace(".", "_", StringComparison.Ordinal)
            .Replace("+", "_", StringComparison.Ordinal);
    });
});
builder.Services.AddMemoryCache();

builder.Services.AddOptions<CBInsightsOptions>()
    .Bind(builder.Configuration.GetSection("CBInsights"))
    .ValidateDataAnnotations()
    .Validate(options => !string.Equals(options.ClientId, "CHANGE-ME", StringComparison.OrdinalIgnoreCase), "CB Insights client id must be configured.")
    .Validate(options => !string.Equals(options.ClientSecret, "CHANGE-ME", StringComparison.OrdinalIgnoreCase), "CB Insights client secret must be configured.")
    .ValidateOnStart();

builder.Services.AddSingleton<ILogMessageProvider, LogMessageProvider>();
builder.Services.AddSingleton<IErrorMessageProvider, ErrorMessageProvider>();
builder.Services.AddSingleton<ICBInsightsTokenProvider, CBInsightsTokenProvider>();
builder.Services.AddSingleton<ICBInsightsAuthenticationProvider, CBInsightsAuthenticationProvider>();
builder.Services.AddSingleton<ICBInsightsRequestAdapterFactory, CBInsightsRequestAdapterFactory>();
builder.Services.AddScoped<ICBInsightsClient, CBInsightsClient>();
builder.Services.AddScoped<IOrganizationLookupCommandHandler, OrganizationLookupCommandHandler>();
builder.Services.AddScoped<IFirmographicsCommandHandler, FirmographicsCommandHandler>();
builder.Services.AddScoped<IFundingsCommandHandler, FundingsCommandHandler>();
builder.Services.AddScoped<IInvestmentsCommandHandler, InvestmentsCommandHandler>();
builder.Services.AddScoped<IPortfolioExitsCommandHandler, PortfolioExitsCommandHandler>();
builder.Services.AddScoped<IBusinessRelationshipsCommandHandler, BusinessRelationshipsCommandHandler>();
builder.Services.AddScoped<IManagementAndBoardCommandHandler, ManagementAndBoardCommandHandler>();
builder.Services.AddScoped<IOutlookCommandHandler, OutlookCommandHandler>();
builder.Services.AddScoped<ICommercialMaturityCommandHandler, CommercialMaturityCommandHandler>();
builder.Services.AddScoped<IExitProbabilityCommandHandler, ExitProbabilityCommandHandler>();
builder.Services.AddScoped<IMosaicScoreCommandHandler, MosaicScoreCommandHandler>();
builder.Services.AddScoped<IScoutingReportCommandHandler, ScoutingReportCommandHandler>();
builder.Services.AddScoped<IScoutingReportStreamCommandHandler, ScoutingReportStreamCommandHandler>();
builder.Services.AddScoped<IChatCbiCommandHandler, ChatCbiCommandHandler>();
builder.Services.AddScoped<IChatCbiStreamCommandHandler, ChatCbiStreamCommandHandler>();

builder.Services.AddHttpClient(CBInsightsHttpClientNames.Authorization, (provider, client) =>
{
    var options = provider.GetRequiredService<IOptionsMonitor<CBInsightsOptions>>().CurrentValue;
    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(PollyPolicies.CreateRetryPolicy())
.AddPolicyHandler(PollyPolicies.CreateCircuitBreakerPolicy());

builder.Services.AddHttpClient(CBInsightsHttpClientNames.Api, (provider, client) =>
{
    var options = provider.GetRequiredService<IOptionsMonitor<CBInsightsOptions>>().CurrentValue;
    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(PollyPolicies.CreateRetryPolicy())
.AddPolicyHandler(PollyPolicies.CreateCircuitBreakerPolicy());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            var servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "https://api.example.com/v1", // Production API URL
                    Description = "Production Server (Live Data)"
                },
                new()
                {
                    Url = "https://sandbox-api.example.com:8443/v1", // QA API URL
                    Description = "Sandbox Server (Test Data)"
                }
            };

            var scheme = httpReq.Headers.TryGetValue("X-Forwarded-Proto", out var forwardedProto) && !StringValues.IsNullOrEmpty(forwardedProto)
                ? forwardedProto.ToString()
                : httpReq.Scheme;

            var host = httpReq.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHost) && !StringValues.IsNullOrEmpty(forwardedHost)
                ? forwardedHost.ToString()
                : httpReq.Host.Host;

            var port = httpReq.Headers.TryGetValue("X-Forwarded-Port", out var forwardedPort) && !StringValues.IsNullOrEmpty(forwardedPort)
                ? forwardedPort.ToString()
                : httpReq.Host.Port?.ToString(CultureInfo.InvariantCulture);

            var hostWithPort = !string.IsNullOrWhiteSpace(host)
                ? host
                : httpReq.Host.Value;

            if (!string.IsNullOrWhiteSpace(port) && host is not null && !host.Contains(':', StringComparison.Ordinal))
            {
                hostWithPort = $"{host}:{port}";
            }

            var requestUrl = string.IsNullOrWhiteSpace(hostWithPort)
                ? null
                : $"{scheme}://{hostWithPort}";

            var comparisonHost = host ?? httpReq.Host.Host;
            var isDevTunnelHost = !string.IsNullOrWhiteSpace(comparisonHost) && comparisonHost.Contains(".devtunnels.ms", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(requestUrl))
            {
                servers.Add(new()
                {
                    Url = requestUrl,
                    Description = isDevTunnelHost ? "Development Server (Visual Studio Dev Tunnel)" : "Development Server (Local Host)"
                });
            }

            var devTunnelUrl = Environment.GetEnvironmentVariable("VS_TUNNEL_URL");
            if (!string.IsNullOrWhiteSpace(devTunnelUrl) && !string.Equals(devTunnelUrl, requestUrl, StringComparison.OrdinalIgnoreCase))
            {
                servers.Add(new()
                {
                    Url = devTunnelUrl,
                    Description = "Development Server (Visual Studio Dev Tunnel)"
                });
            }

            swaggerDoc.Servers = servers;
        });
    });
    app.UseSwaggerUI(static options =>
    {
        options.DocumentTitle = "TearLogic CB Insights API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TearLogic CB Insights API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
