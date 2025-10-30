using System.IO;
using System.Reflection;
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
    app.UseSwagger();
    app.UseSwaggerUI(static options =>
    {
        options.DocumentTitle = "TearLogic CB Insights API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TearLogic CB Insights API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
