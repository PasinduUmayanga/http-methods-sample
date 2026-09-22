using Inventory.DTOs;

namespace Inventory.Application;

/// <summary>
/// Defines inventory business operations used by HTTP handlers.
/// </summary>
public interface IInventoryService
{
    IReadOnlyCollection<InventoryItem> GetAll();

    InventoryItem? GetById(int id);

    InventoryItem Create(string name, int quantity);

    InventoryItem Replace(int id, string name, int quantity);

    InventoryItem? Patch(int id, string? name, int? quantity);

    bool Delete(int id);

    bool Exists(int id);
}
