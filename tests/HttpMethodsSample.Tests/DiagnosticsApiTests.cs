using System.Net;
using System.Net.Http.Json;
using Diagnostics.Api;
using Microsoft.AspNetCore.Routing;

namespace HttpMethodsSample.Tests;

public sealed class DiagnosticsApiTests
{
    [Fact]
    public async Task GetDiagnosticsReturnsSupportedMethods()
    {
        await using var app = await TestApplication.StartDiagnosticsAsync();

        var response = await app.Client.GetAsync("/diagnostics");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("GET", body);
        Assert.Contains("OPTIONS", body);
        Assert.Contains("TRACE", body);
        Assert.Contains("CONNECT", body);
    }

    [Fact]
    public async Task OptionsDiagnosticsReturnsSupportedDiagnosticsMethods()
    {
        await using var app = await TestApplication.StartDiagnosticsAsync();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/diagnostics");

        var response = await app.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("GET, OPTIONS, TRACE, CONNECT", response.Content.Headers.Allow.ToString());
    }

    [Fact]
    public async Task TraceDiagnosticsEchoesRequestMetadataAndRedactsSensitiveHeaders()
    {
        await using var app = await TestApplication.StartDiagnosticsAsync();
        using var request = new HttpRequestMessage(new HttpMethod("TRACE"), "/diagnostics/trace?demo=true");
        request.Headers.Add("Authorization", "Bearer secret");
        request.Headers.Add("X-Demo", "visible");

        var response = await app.Client.SendAsync(request);
        var echo = await response.Content.ReadFromJsonAsync<TraceEcho>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(echo);
        Assert.Equal("TRACE", echo.Method);
        Assert.Equal("/diagnostics/trace", echo.Path);
        Assert.Equal("?demo=true", echo.QueryString);
        Assert.Equal("[redacted]", echo.Headers["Authorization"]);
        Assert.Equal("visible", echo.Headers["X-Demo"]);
    }

    [Fact]
    public async Task ConnectDiagnosticsReturnsDemoResponseWithoutOpeningTunnel()
    {
        await using var app = await TestApplication.StartDiagnosticsAsync();
        var response = await app.Client.PostAsync("/diagnostics/connect/example.com:443", null);
        var demo = await response.Content.ReadFromJsonAsync<ConnectDemo>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(demo);
        Assert.Equal("example.com:443", demo.Authority);
        Assert.False(demo.TunnelEstablished);
    }

    [Fact]
    public async Task ConnectDiagnosticsRouteIsMappedForTheActualHttpMethod()
    {
        await using var app = await TestApplication.StartDiagnosticsAsync();

        var endpoint = app.Endpoints
            .Endpoints
            .OfType<RouteEndpoint>()
            .Single(route => route.Metadata
                .GetMetadata<HttpMethodMetadata>()?
                .HttpMethods
                .Contains("CONNECT") == true);

        var methods = endpoint.Metadata
            .GetRequiredMetadata<HttpMethodMetadata>()
            .HttpMethods;

        Assert.Contains("CONNECT", methods);
        Assert.Contains("POST", methods);
    }
}
