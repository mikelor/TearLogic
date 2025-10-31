using System.Collections.Generic;
using System.Linq;
using TearLogic.Clients.Models.V2ManagementAndBoard;

namespace TearLogic.Api.CBInsights.Requests;

/// <summary>
/// Represents filters applied when retrieving management and board information for an organization.
/// </summary>
public sealed class ManagementAndBoardRequest
{
    /// <summary>
    /// Gets or sets the collection of title identifiers used to filter the response.
    /// </summary>
    public ICollection<int>? TitleIds { get; set; }

    /// <summary>
    /// Converts the request to the Kiota model used by the CB Insights client.
    /// </summary>
    /// <returns>The populated Kiota request model.</returns>
    public ManagementAndBoardRequestBody ToKiotaModel()
    {
        var requestBody = new ManagementAndBoardRequestBody();
        if (TitleIds is { Count: > 0 })
        {
            requestBody.TitleIds = TitleIds
                .Where(id => id > 0)
                .Select(static id => (int?)id)
                .ToList();
        }

        return requestBody;
    }
}
