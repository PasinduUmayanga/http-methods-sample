namespace Diagnostics.DTOs;

/// <summary>
/// Response payload returned by the TRACE endpoint after sensitive headers are redacted.
/// </summary>
public sealed record TraceEcho(
    string Method,
    string Scheme,
    string Host,
    string Path,
    string QueryString,
    IReadOnlyDictionary<string, string> Headers);
