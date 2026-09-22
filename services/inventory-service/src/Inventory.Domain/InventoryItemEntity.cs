namespace Inventory.Domain;

/// <summary>
/// Core inventory entity used by the application and infrastructure layers.
/// </summary>
public sealed record InventoryItemEntity(int Id, string Name, int Quantity);
