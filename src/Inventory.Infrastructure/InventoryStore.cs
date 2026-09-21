using System.Collections.Concurrent;
using Inventory.DTOs;

namespace Inventory.Infrastructure;

/// <summary>
/// Thread-safe in-memory store used by the Inventory API sample instead of a database.
/// </summary>
public sealed class InventoryStore
{
    private readonly ConcurrentDictionary<int, InventoryItem> items = new();
    private int nextId = 1;

    public InventoryStore()
    {
        items[1] = new InventoryItem(1, "Sample notebook", 12);
    }

    public IReadOnlyCollection<InventoryItem> All() =>
        items.Values.OrderBy(item => item.Id).ToArray();

    public InventoryItem? Find(int id) =>
        items.GetValueOrDefault(id);

    public InventoryItem Create(string name, int quantity)
    {
        var id = Interlocked.Increment(ref nextId);
        var item = new InventoryItem(id, name, quantity);
        items[id] = item;
        return item;
    }

    public InventoryItem Replace(int id, string name, int quantity)
    {
        var item = new InventoryItem(id, name, quantity);
        items[id] = item;
        return item;
    }

    public InventoryItem? Patch(int id, string? name, int? quantity)
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
