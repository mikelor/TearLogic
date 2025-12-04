namespace TearLogic.Api.CBInsights.Diagnostics;

/// <summary>
/// Provides high-performance logging extensions using source generators.
/// </summary>
public static partial class LoggerExtensions
{
    // Organization Lookup
    [LoggerMessage(Level = LogLevel.Information, Message = "Organization lookup started")]
    public static partial void LogOrganizationLookupStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Organization lookup completed")]
    public static partial void LogOrganizationLookupCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Organization lookup failed. Code: {ErrorCode}")]
    public static partial void LogOrganizationLookupFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Organization lookup failed")]
    public static partial void LogOrganizationLookupFailedGeneric(this ILogger logger, Exception exception);

    // Firmographics
    [LoggerMessage(Level = LogLevel.Information, Message = "Firmographics lookup started")]
    public static partial void LogFirmographicsLookupStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Firmographics lookup completed")]
    public static partial void LogFirmographicsLookupCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Firmographics lookup failed. Code: {ErrorCode}")]
    public static partial void LogFirmographicsLookupFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Firmographics lookup failed")]
    public static partial void LogFirmographicsLookupFailedGeneric(this ILogger logger, Exception exception);

    // Fundings
    [LoggerMessage(Level = LogLevel.Information, Message = "Fundings request started")]
    public static partial void LogFundingsRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Fundings request completed")]
    public static partial void LogFundingsRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Fundings request failed. Code: {ErrorCode}")]
    public static partial void LogFundingsRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Fundings request failed")]
    public static partial void LogFundingsRequestFailedGeneric(this ILogger logger, Exception exception);

    // Investments
    [LoggerMessage(Level = LogLevel.Information, Message = "Investments request started")]
    public static partial void LogInvestmentsRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Investments request completed")]
    public static partial void LogInvestmentsRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Investments request failed. Code: {ErrorCode}")]
    public static partial void LogInvestmentsRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Investments request failed")]
    public static partial void LogInvestmentsRequestFailedGeneric(this ILogger logger, Exception exception);

    // Portfolio Exits
    [LoggerMessage(Level = LogLevel.Information, Message = "Portfolio exits request started")]
    public static partial void LogPortfolioExitsRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Portfolio exits request completed")]
    public static partial void LogPortfolioExitsRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Portfolio exits request failed. Code: {ErrorCode}")]
    public static partial void LogPortfolioExitsRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Portfolio exits request failed")]
    public static partial void LogPortfolioExitsRequestFailedGeneric(this ILogger logger, Exception exception);

    // Business Relationships
    [LoggerMessage(Level = LogLevel.Information, Message = "Business relationships request started")]
    public static partial void LogBusinessRelationshipsRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Business relationships request completed")]
    public static partial void LogBusinessRelationshipsRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Business relationships request failed. Code: {ErrorCode}")]
    public static partial void LogBusinessRelationshipsRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Business relationships request failed")]
    public static partial void LogBusinessRelationshipsRequestFailedGeneric(this ILogger logger, Exception exception);

    // Management and Board
    [LoggerMessage(Level = LogLevel.Information, Message = "Management and board request started")]
    public static partial void LogManagementAndBoardRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Management and board request completed")]
    public static partial void LogManagementAndBoardRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Management and board request failed. Code: {ErrorCode}")]
    public static partial void LogManagementAndBoardRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Management and board request failed")]
    public static partial void LogManagementAndBoardRequestFailedGeneric(this ILogger logger, Exception exception);

    // Outlook
    [LoggerMessage(Level = LogLevel.Information, Message = "Outlook request started")]
    public static partial void LogOutlookRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outlook request completed")]
    public static partial void LogOutlookRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Outlook request failed. Code: {ErrorCode}")]
    public static partial void LogOutlookRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Outlook request failed")]
    public static partial void LogOutlookRequestFailedGeneric(this ILogger logger, Exception exception);

    // Scouting Report
    [LoggerMessage(Level = LogLevel.Information, Message = "Scouting report request started")]
    public static partial void LogScoutingReportRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Scouting report request completed")]
    public static partial void LogScoutingReportRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Scouting report request failed. Code: {ErrorCode}")]
    public static partial void LogScoutingReportRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Scouting report request failed")]
    public static partial void LogScoutingReportRequestFailedGeneric(this ILogger logger, Exception exception);

    // Scouting Report Stream
    [LoggerMessage(Level = LogLevel.Information, Message = "Scouting report stream request started")]
    public static partial void LogScoutingReportStreamRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Scouting report stream request completed")]
    public static partial void LogScoutingReportStreamRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Scouting report stream request failed. Code: {ErrorCode}")]
    public static partial void LogScoutingReportStreamRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Scouting report stream request failed")]
    public static partial void LogScoutingReportStreamRequestFailedGeneric(this ILogger logger, Exception exception);

    // ChatCBI
    [LoggerMessage(Level = LogLevel.Information, Message = "ChatCBI request started")]
    public static partial void LogChatCbiRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "ChatCBI request completed")]
    public static partial void LogChatCbiRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "ChatCBI request failed. Code: {ErrorCode}")]
    public static partial void LogChatCbiRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "ChatCBI request failed")]
    public static partial void LogChatCbiRequestFailedGeneric(this ILogger logger, Exception exception);

    // ChatCBI Stream
    [LoggerMessage(Level = LogLevel.Information, Message = "ChatCBI stream request started")]
    public static partial void LogChatCbiStreamRequestStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "ChatCBI stream request completed")]
    public static partial void LogChatCbiStreamRequestCompleted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "ChatCBI stream request failed. Code: {ErrorCode}")]
    public static partial void LogChatCbiStreamRequestFailed(this ILogger logger, Exception exception, string errorCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "ChatCBI stream request failed")]
    public static partial void LogChatCbiStreamRequestFailedGeneric(this ILogger logger, Exception exception);

    // Token Provider
    [LoggerMessage(Level = LogLevel.Information, Message = "Token cache hit")]
    public static partial void LogTokenCacheHit(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Token refreshing")]
    public static partial void LogTokenRefreshing(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Token acquisition failed. StatusCode: {StatusCode}")]
    public static partial void LogTokenAcquisitionFailed(this ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Token response invalid")]
    public static partial void LogTokenResponseInvalid(this ILogger logger);
}
