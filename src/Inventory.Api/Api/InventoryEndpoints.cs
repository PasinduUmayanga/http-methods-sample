using Inventory.Application;
using Inventory.Infrastructure;

namespace Inventory.Api.Api;

/// <summary>
/// Provides service registration and endpoint mappings for the Inventory API sample.
/// </summary>
public static class InventoryEndpoints
{
    /// <summary>
    /// Registers services required by the Inventory microservice.
    /// </summary>
    public static IServiceCollection AddInventorySample(this IServiceCollection services)
    {
        services.AddSingleton<InventoryStore>();
        services.AddSingleton<IInventoryService, InventoryService>();
        return services;
    }

    /// <summary>
    /// Maps all Inventory API routes that demonstrate GET, POST, PUT, PATCH, DELETE, HEAD, and OPTIONS.
    /// </summary>
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", InventoryHandlers.GetApiInfo);

        // GET /inventory
        endpoints.MapGet(InventoryRoutes.Collection, InventoryHandlers.GetAll);

        // POST /inventory
        endpoints.MapPost(InventoryRoutes.Collection, InventoryHandlers.Create);

        // OPTIONS /inventory
        endpoints.MapMethods(InventoryRoutes.Collection, ["OPTIONS"], (HttpContext context) =>
            InventoryHandlers.AllowedMethods(context, InventoryRoutes.CollectionMethods));

        var items = endpoints.MapGroup(InventoryRoutes.Collection);

        // GET /inventory/{id}
        items.MapGet(InventoryRoutes.ItemById, InventoryHandlers.GetById);

        // PUT /inventory/{id}
        items.MapPut(InventoryRoutes.ItemById, InventoryHandlers.Replace);

        // PATCH /inventory/{id}
        items.MapPatch(InventoryRoutes.ItemById, InventoryHandlers.Patch);

        // DELETE /inventory/{id}
        items.MapDelete(InventoryRoutes.ItemById, InventoryHandlers.Delete);

        // HEAD /inventory/{id}
        items.MapMethods(InventoryRoutes.ItemById, ["HEAD"], InventoryHandlers.Head);

        // OPTIONS /inventory/{id}
        items.MapMethods(InventoryRoutes.ItemById, ["OPTIONS"], (HttpContext context) =>
            InventoryHandlers.AllowedMethods(context, InventoryRoutes.ItemMethods));

        return endpoints;
    }
}
