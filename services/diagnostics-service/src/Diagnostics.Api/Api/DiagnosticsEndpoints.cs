using Diagnostics.Application;

namespace Diagnostics.Api.Api;

/// <summary>
/// Provides service registration and endpoint mappings for the Diagnostics API sample.
/// </summary>
public static class DiagnosticsEndpoints
{
    /// <summary>
    /// Registers services required by the Diagnostics microservice.
    /// </summary>
    public static IServiceCollection AddDiagnosticsSample(this IServiceCollection services)
    {
        services.AddSingleton<IDiagnosticsService, DiagnosticsService>();
        return services;
    }

    /// <summary>
    /// Maps all Diagnostics API routes used to demonstrate GET, OPTIONS, TRACE, and CONNECT.
    /// </summary>
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", DiagnosticsHandlers.GetApiInfo);

        // GET /diagnostics
        endpoints.MapGet(DiagnosticsRoutes.Root, DiagnosticsHandlers.GetOverview);

        // OPTIONS /diagnostics
        endpoints.MapMethods(DiagnosticsRoutes.Root, ["OPTIONS"], DiagnosticsHandlers.AllowedMethods);

        // TRACE /diagnostics/trace
        endpoints.MapMethods(DiagnosticsRoutes.Trace, ["TRACE"], DiagnosticsHandlers.Trace);

        // CONNECT /diagnostics/connect/{authority}
        endpoints.MapMethods(DiagnosticsRoutes.Connect, ["CONNECT", "POST"], DiagnosticsHandlers.Connect);

        return endpoints;
    }
}
