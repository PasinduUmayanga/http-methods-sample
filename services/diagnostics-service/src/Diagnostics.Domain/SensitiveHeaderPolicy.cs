namespace Diagnostics.Domain;

/// <summary>
/// Domain policy that identifies headers that must not be echoed back in TRACE responses.
/// </summary>
public static class SensitiveHeaderPolicy
{
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Proxy-Authorization",
        "Set-Cookie"
    };

    public static bool IsSensitive(string headerName) =>
        SensitiveHeaders.Contains(headerName);
}
