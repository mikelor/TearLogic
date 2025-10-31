using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace TearLogic.Api.CBInsights.Controllers;

/// <summary>
/// Provides validation helpers for API controllers.
/// </summary>
internal static class ControllerValidationExtensions
{
    /// <summary>
    /// Validates an optional CB Insights organization identifier and records model errors when invalid.
    /// </summary>
    /// <param name="controller">The controller requesting validation.</param>
    /// <param name="organizationId">The optional identifier to validate.</param>
    /// <param name="validatedOrganizationId">When the method returns <c>true</c>, contains the validated identifier.</param>
    /// <returns><c>true</c> when the identifier is present and greater than zero; otherwise, <c>false</c>.</returns>
    public static bool TryValidateOrganizationId(
        this ControllerBase controller,
        int? organizationId,
        [NotNullWhen(true)] out int validatedOrganizationId)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (!organizationId.HasValue || organizationId.Value <= 0)
        {
            controller.ModelState.AddModelError(nameof(organizationId), "The organization identifier must be a positive integer.");
            validatedOrganizationId = default;
            return false;
        }

        validatedOrganizationId = organizationId.Value;
        return true;
    }
}
