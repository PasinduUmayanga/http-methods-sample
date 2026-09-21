using Diagnostics.DTOs;
using Microsoft.AspNetCore.Http;

namespace Diagnostics.Application;

/// <summary>
/// Coordinates protocol diagnostic behavior for the Diagnostics microservice.
/// </summary>
public sealed class DiagnosticsService : IDiagnosticsService
{
    private static readonly string[] Methods = ["GET", "OPTIONS", "TRACE", "CONNECT"];

    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Proxy-Authorization",
        "Set-Cookie"
    };

    public DiagnosticsOverview GetOverview() =>
        new(
            "Use this service to inspect HTTP metadata and learn about less common methods.",
            Methods);

    public TraceEcho CreateTraceEcho(HttpRequest request)
    {
        var headers = request.Headers
            .OrderBy(header => header.Key)
            .ToDictionary(
                header => header.Key,
                header => SensitiveHeaders.Contains(header.Key) ? "[redacted]" : header.Value.ToString());

        return new TraceEcho(
            request.Method,
            request.Scheme,
            request.Host.ToString(),
            request.Path,
            request.QueryString.ToString(),
            headers);
    }

    public ConnectDemo CreateConnectDemo(string authority) =>
        new(
            authority,
            "CONNECT normally asks an HTTP proxy to open a tunnel. This sample acknowledges the request but does not create a network tunnel.",
            false);
}
