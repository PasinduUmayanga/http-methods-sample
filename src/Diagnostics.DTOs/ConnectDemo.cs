namespace Diagnostics.DTOs;

/// <summary>
/// Response payload returned by the safe CONNECT demonstration endpoint.
/// </summary>
public sealed record ConnectDemo(
    string Authority,
    string Explanation,
    bool TunnelEstablished);
