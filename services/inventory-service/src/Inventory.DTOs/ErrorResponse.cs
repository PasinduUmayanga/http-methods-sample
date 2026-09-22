namespace Inventory.DTOs;

/// <summary>
/// Simple error payload returned when a request is invalid or a resource is missing.
/// </summary>
public sealed record ErrorResponse(string Error);
