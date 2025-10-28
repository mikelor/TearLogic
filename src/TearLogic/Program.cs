namespace TearLogic;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TearLogic.Clients;
using TearLogic.Clients.Models.V2OrganizationLookup;

internal class Program
{
    static async Task Main(string[] args)
    {
        // API requires no authentication, so use the anonymous
        // authentication provider
        var authProvider = new AnonymousAuthenticationProvider();
        // Create request adapter using the HttpClient-based implementation
        var adapter = new HttpClientRequestAdapter(authProvider);
        // Create the API client
        var client = new CBInsightsApiClient(adapter);

        // There is no GetAsync method. Use PostAsync with appropriate request body.
        var requestBody = new OrgLookupRequestBody();
        var allFirms = await client.V2.Organizations.PostAsync(requestBody);
    }
}