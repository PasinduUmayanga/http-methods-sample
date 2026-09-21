namespace Diagnostics.Api;

public static class DiagnosticsEndpoints
{
    private static readonly string[] Methods = ["GET", "OPTIONS", "TRACE", "CONNECT"];
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Proxy-Authorization",
        "Set-Cookie"
    };

    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => Results.Ok(new
        {
            service = "Diagnostics API",
            description = "Protocol-oriented sample for OPTIONS, TRACE, and CONNECT.",
            routes = new[] { "/diagnostics", "/diagnostics/trace", "/diagnostics/connect/{authority}" }
        }));

        // GET returns a read-only overview of the diagnostic methods in this service.
        endpoints.MapGet("/diagnostics", () => Results.Ok(new
        {
            message = "Use this service to inspect HTTP metadata and learn about less common methods.",
            supportedMethods = Methods
        }));

        // OPTIONS advertises the diagnostic methods supported by this endpoint.
        endpoints.MapMethods("/diagnostics", ["OPTIONS"], () =>
            Results.NoContent().WithHeader("Allow", string.Join(", ", Methods)));

        // TRACE echoes request metadata for diagnostics while redacting sensitive headers.
        endpoints.MapMethods("/diagnostics/trace", ["TRACE"], (HttpContext context) =>
        {
            var headers = context.Request.Headers
                .OrderBy(header => header.Key)
                .ToDictionary(
                    header => header.Key,
                    header => SensitiveHeaders.Contains(header.Key) ? "[redacted]" : header.Value.ToString());

            return Results.Ok(new TraceEcho(
                context.Request.Method,
                context.Request.Scheme,
                context.Request.Host.ToString(),
                context.Request.Path,
                context.Request.QueryString.ToString(),
                headers));
        });

        // CONNECT normally establishes proxy tunnels; this safe sample acknowledges the request only.
        // POST mirrors the same response so regular clients and CI can test it without proxy tunneling.
        endpoints.MapMethods("/diagnostics/connect/{authority}", ["CONNECT", "POST"], (string authority) =>
            Results.Ok(CreateConnectDemo(authority)));

        return endpoints;
    }

    public static ConnectDemo CreateConnectDemo(string authority) =>
        new(
            authority,
            "CONNECT normally asks an HTTP proxy to open a tunnel. This sample acknowledges the request but does not create a network tunnel.",
            false);
}

public sealed record TraceEcho(
    string Method,
    string Scheme,
    string Host,
    string Path,
    string QueryString,
    IReadOnlyDictionary<string, string> Headers);

public sealed record ConnectDemo(
    string Authority,
    string Explanation,
    bool TunnelEstablished);

internal static class ResultHeaderExtensions
{
    public static IResult WithHeader(this IResult result, string name, string value) =>
        new HeaderResult(result, name, value);

    private sealed class HeaderResult(IResult inner, string name, string value) : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.Headers[name] = value;
            await inner.ExecuteAsync(httpContext);
        }
    }
}
