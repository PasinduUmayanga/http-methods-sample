using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Api;

public static class InventoryEndpoints
{
    private const string InventoryRoute = "/inventory";
    private const string InventoryItemRoute = $"{InventoryRoute}/{{id:int}}";

    private static readonly string[] ItemMethods = ["GET", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];
    private static readonly string[] CollectionMethods = ["GET", "POST", "OPTIONS"];

    public static IServiceCollection AddInventorySample(this IServiceCollection services)
    {
        services.AddSingleton<InventoryStore>();
        return services;
    }

    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => Results.Ok(new
        {
            service = "Inventory API",
            description = "CRUD-style sample for GET, POST, PUT, PATCH, DELETE, HEAD, and OPTIONS.",
            routes = new[] { InventoryRoute, "/inventory/{id}" }
        }));

        // GET reads the current collection without changing server state.
        endpoints.MapGet(InventoryRoute, (InventoryStore store) =>
            Results.Ok(store.All()));

        // POST creates a new inventory item and returns 201 Created with its Location.
        endpoints.MapPost(InventoryRoute, (CreateInventoryItemRequest request, InventoryStore store, HttpContext context) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new ErrorResponse("Name is required."));
            }

            var item = store.Create(request.Name.Trim(), request.Quantity);
            var location = $"{context.Request.Scheme}://{context.Request.Host}/inventory/{item.Id}";
            return Results.Created(location, item);
        });

        // OPTIONS advertises which methods the collection endpoint supports.
        endpoints.MapMethods(InventoryRoute, ["OPTIONS"], () =>
        {
            return Results.NoContent().WithHeader("Allow", string.Join(", ", CollectionMethods));
        });

        // GET reads one resource by identifier and returns 404 when it is missing.
        endpoints.MapGet(InventoryItemRoute, Results<Ok<InventoryItem>, NotFound<ErrorResponse>> (int id, InventoryStore store) =>
        {
            var item = store.Find(id);
            return item is null
                ? TypedResults.NotFound(new ErrorResponse($"Inventory item {id} was not found."))
                : TypedResults.Ok(item);
        });

        // PUT replaces the full resource at this URI, creating it when needed.
        endpoints.MapPut(InventoryItemRoute, (int id, CreateInventoryItemRequest request, InventoryStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new ErrorResponse("Name is required."));
            }

            var item = store.Replace(id, request.Name.Trim(), request.Quantity);
            return Results.Ok(item);
        });

        // PATCH changes only the fields supplied in the request body.
        endpoints.MapPatch(InventoryItemRoute, Results<Ok<InventoryItem>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>> (int id, PatchInventoryItemRequest request, InventoryStore store) =>
        {
            if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
            {
                return TypedResults.BadRequest(new ErrorResponse("Name cannot be blank when supplied."));
            }

            var item = store.Patch(id, request.Name?.Trim(), request.Quantity);
            return item is null
                ? TypedResults.NotFound(new ErrorResponse($"Inventory item {id} was not found."))
                : TypedResults.Ok(item);
        });

        // DELETE removes the resource and returns 204 when the deletion succeeds.
        endpoints.MapDelete(InventoryItemRoute, (int id, InventoryStore store) =>
            store.Delete(id)
                ? Results.NoContent()
                : Results.NotFound(new ErrorResponse($"Inventory item {id} was not found.")));

        // HEAD returns metadata for the resource without writing a response body.
        endpoints.MapMethods(InventoryItemRoute, ["HEAD"], (int id, InventoryStore store, HttpContext context) =>
        {
            if (store.Find(id) is null)
            {
                return Results.NotFound();
            }

            context.Response.Headers.ContentType = "application/json";
            context.Response.Headers.ETag = $"\"inventory-{id}\"";
            return Results.Ok();
        });

        // OPTIONS advertises which methods an individual item endpoint supports.
        endpoints.MapMethods(InventoryItemRoute, ["OPTIONS"], () =>
        {
            return Results.NoContent().WithHeader("Allow", string.Join(", ", ItemMethods));
        });

        return endpoints;
    }
}

public sealed record InventoryItem(int Id, string Name, int Quantity);

public sealed record CreateInventoryItemRequest(string Name, int Quantity);

public sealed record PatchInventoryItemRequest(string? Name, int? Quantity);

public sealed record ErrorResponse(string Error);

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

internal static class ResultHeaderExtensions
{
    public static IResult WithHeader(this IResult result, string name, string value) =>
        new HeaderResult(result, name, value);

    private sealed class HeaderResult(IResult inner, string name, string value) : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.Headers[name] = value;
            await inner.ExecuteAsync(httpContext);
        }
    }
}
