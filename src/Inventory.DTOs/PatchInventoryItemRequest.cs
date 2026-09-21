namespace Inventory.DTOs;

/// <summary>
/// Request body used by PATCH when partially updating an inventory item.
/// Null properties mean the caller does not want to change that field.
/// </summary>
public sealed record PatchInventoryItemRequest(string? Name, int? Quantity);
