using Inventory.Domain;

namespace Inventory.Application;

/// <summary>
/// Application port for inventory persistence. Infrastructure implements this interface.
/// </summary>
public interface IInventoryRepository
{
    IReadOnlyCollection<InventoryItemEntity> GetAll();

    InventoryItemEntity? GetById(int id);

    InventoryItemEntity Create(string name, int quantity);

    InventoryItemEntity Replace(int id, string name, int quantity);

    InventoryItemEntity? Patch(int id, string? name, int? quantity);

    bool Delete(int id);
}
