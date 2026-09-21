using Diagnostics.Application;

namespace Diagnostics.Api.Api;

/// <summary>
/// Contains HTTP handlers for the Diagnostics API layer.
/// </summary>
internal static class DiagnosticsHandlers
{
    public static IResult GetApiInfo() =>
        Results.Ok(new
        {
            service = "Diagnostics API",
            description = "Protocol-oriented sample for OPTIONS, TRACE, and CONNECT.",
            routes = new[] { DiagnosticsRoutes.Root, DiagnosticsRoutes.Trace, DiagnosticsRoutes.Connect }
        });

    public static IResult GetOverview(IDiagnosticsService diagnostics) =>
        Results.Ok(diagnostics.GetOverview());

    public static IResult AllowedMethods(HttpContext context)
    {
        context.Response.Headers.Allow = string.Join(", ", DiagnosticsRoutes.Methods);
        return Results.NoContent();
    }

    public static IResult Trace(HttpContext context, IDiagnosticsService diagnostics) =>
        Results.Ok(diagnostics.CreateTraceEcho(context.Request));

    public static IResult Connect(string authority, IDiagnosticsService diagnostics) =>
        Results.Ok(diagnostics.CreateConnectDemo(authority));
}
