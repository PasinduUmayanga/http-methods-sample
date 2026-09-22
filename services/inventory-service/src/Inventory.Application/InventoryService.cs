using Inventory.DTOs;
using Inventory.Domain;

namespace Inventory.Application;

/// <summary>
/// Coordinates inventory use cases for the Inventory microservice.
/// </summary>
public sealed class InventoryService(IInventoryRepository repository) : IInventoryService
{
    public IReadOnlyCollection<InventoryItem> GetAll() =>
        repository.GetAll().Select(ToDto).ToArray();

    public InventoryItem? GetById(int id) =>
        ToNullableDto(repository.GetById(id));

    public InventoryItem Create(string name, int quantity) =>
        ToDto(repository.Create(name, quantity));

    public InventoryItem Replace(int id, string name, int quantity) =>
        ToDto(repository.Replace(id, name, quantity));

    public InventoryItem? Patch(int id, string? name, int? quantity) =>
        ToNullableDto(repository.Patch(id, name, quantity));

    public bool Delete(int id) =>
        repository.Delete(id);

    public bool Exists(int id) =>
        repository.GetById(id) is not null;

    private static InventoryItem ToDto(InventoryItemEntity entity) =>
        new(entity.Id, entity.Name, entity.Quantity);

    private static InventoryItem? ToNullableDto(InventoryItemEntity? entity) =>
        entity is null ? null : ToDto(entity);
}
