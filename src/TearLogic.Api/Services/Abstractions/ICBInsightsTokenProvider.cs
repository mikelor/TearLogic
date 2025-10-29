namespace TearLogic.Api.Services.Abstractions;

public interface ICBInsightsTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}
