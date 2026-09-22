namespace Diagnostics.DTOs;

/// <summary>
/// Response payload describing the Diagnostics microservice.
/// </summary>
public sealed record DiagnosticsOverview(string Message, IReadOnlyCollection<string> SupportedMethods);
