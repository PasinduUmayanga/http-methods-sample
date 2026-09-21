namespace Inventory.DTOs;

/// <summary>
/// Represents one inventory resource returned by the sample API.
/// </summary>
public sealed record InventoryItem(int Id, string Name, int Quantity);
