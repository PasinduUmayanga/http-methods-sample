using Inventory.DTOs;
using Inventory.Infrastructure;

namespace Inventory.Application;

/// <summary>
/// Coordinates inventory use cases for the Inventory microservice.
/// </summary>
public sealed class InventoryService(InventoryStore store) : IInventoryService
{
    public IReadOnlyCollection<InventoryItem> GetAll() =>
        store.All();

    public InventoryItem? GetById(int id) =>
        store.Find(id);

    public InventoryItem Create(string name, int quantity) =>
        store.Create(name, quantity);

    public InventoryItem Replace(int id, string name, int quantity) =>
        store.Replace(id, name, quantity);

    public InventoryItem? Patch(int id, string? name, int? quantity) =>
        store.Patch(id, name, quantity);

    public bool Delete(int id) =>
        store.Delete(id);

    public bool Exists(int id) =>
        store.Find(id) is not null;
}
