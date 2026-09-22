namespace Diagnostics.Api.Api;

/// <summary>
/// Centralizes Diagnostics API route templates and Allow-header method lists.
/// </summary>
internal static class DiagnosticsRoutes
{
    public const string Root = "/diagnostics";
    public const string Trace = $"{Root}/trace";
    public const string Connect = $"{Root}/connect/{{authority}}";

    public static readonly string[] Methods = ["GET", "OPTIONS", "TRACE", "CONNECT"];
}
