namespace Inventory.DTOs;

/// <summary>
/// Request body used by POST and PUT when creating or replacing an inventory item.
/// </summary>
public sealed record CreateInventoryItemRequest(string Name, int Quantity);
