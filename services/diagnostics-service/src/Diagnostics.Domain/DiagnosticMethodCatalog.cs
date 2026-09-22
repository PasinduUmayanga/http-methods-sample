namespace Diagnostics.Domain;

/// <summary>
/// Domain-level catalog of diagnostic HTTP methods demonstrated by the service.
/// </summary>
public static class DiagnosticMethodCatalog
{
    public static readonly string[] SupportedMethods = ["GET", "OPTIONS", "TRACE", "CONNECT"];
}
