using Diagnostics.DTOs;
using Microsoft.AspNetCore.Http;

namespace Diagnostics.Application;

/// <summary>
/// Defines protocol-oriented diagnostic use cases used by HTTP handlers.
/// </summary>
public interface IDiagnosticsService
{
    DiagnosticsOverview GetOverview();

    TraceEcho CreateTraceEcho(HttpRequest request);

    ConnectDemo CreateConnectDemo(string authority);
}
