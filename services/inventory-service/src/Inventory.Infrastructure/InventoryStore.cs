using System.Collections.Concurrent;
using Inventory.Application;
using Inventory.Domain;

namespace Inventory.Infrastructure;

/// <summary>
/// Thread-safe in-memory store used by the Inventory API sample instead of a database.
/// </summary>
public sealed class InventoryStore : IInventoryRepository
{
    private readonly ConcurrentDictionary<int, InventoryItemEntity> items = new();
    private int nextId = 1;

    public InventoryStore()
    {
        items[1] = new InventoryItemEntity(1, "Sample notebook", 12);
    }

    public IReadOnlyCollection<InventoryItemEntity> GetAll() =>
        items.Values.OrderBy(item => item.Id).ToArray();

    public InventoryItemEntity? GetById(int id) =>
        items.GetValueOrDefault(id);

    public InventoryItemEntity Create(string name, int quantity)
    {
        var id = Interlocked.Increment(ref nextId);
        var item = new InventoryItemEntity(id, name, quantity);
        items[id] = item;
        return item;
    }

    public InventoryItemEntity Replace(int id, string name, int quantity)
    {
        var item = new InventoryItemEntity(id, name, quantity);
        items[id] = item;
        return item;
    }

    public InventoryItemEntity? Patch(int id, string? name, int? quantity)
    {
        if (!items.TryGetValue(id, out var current))
        {
            return null;
        }

        var updated = current with
        {
            Name = name ?? current.Name,
            Quantity = quantity ?? current.Quantity
        };

        items[id] = updated;
        return updated;
    }

    public bool Delete(int id) =>
        items.TryRemove(id, out _);
}
