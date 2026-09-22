using Inventory.Application;
using Inventory.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Api.Api;

/// <summary>
/// Contains HTTP handlers for the Inventory API layer.
/// </summary>
internal static class InventoryHandlers
{
    public static IResult GetApiInfo() =>
        Results.Ok(new
        {
            service = "Inventory API",
            description = "CRUD-style sample for GET, POST, PUT, PATCH, DELETE, HEAD, and OPTIONS.",
            routes = new[] { InventoryRoutes.Collection, "/inventory/{id}" }
        });

    public static IResult GetAll(IInventoryService inventory) =>
        Results.Ok(inventory.GetAll());

    public static IResult Create(CreateInventoryItemRequest request, IInventoryService inventory, HttpContext context)
    {
        var validationError = ValidateName(request.Name);
        if (validationError is not null)
        {
            return validationError;
        }

        var item = inventory.Create(request.Name.Trim(), request.Quantity);
        var location = $"{context.Request.Scheme}://{context.Request.Host}{InventoryRoutes.Collection}/{item.Id}";
        return Results.Created(location, item);
    }

    public static Results<Ok<InventoryItem>, NotFound<ErrorResponse>> GetById(int id, IInventoryService inventory)
    {
        var item = inventory.GetById(id);
        return item is null
            ? TypedResults.NotFound(ItemNotFound(id))
            : TypedResults.Ok(item);
    }

    public static IResult Replace(int id, CreateInventoryItemRequest request, IInventoryService inventory)
    {
        var validationError = ValidateName(request.Name);
        if (validationError is not null)
        {
            return validationError;
        }

        var item = inventory.Replace(id, request.Name.Trim(), request.Quantity);
        return Results.Ok(item);
    }

    public static Results<Ok<InventoryItem>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>> Patch(
        int id,
        PatchInventoryItemRequest request,
        IInventoryService inventory)
    {
        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.BadRequest(new ErrorResponse("Name cannot be blank when supplied."));
        }

        var item = inventory.Patch(id, request.Name?.Trim(), request.Quantity);
        return item is null
            ? TypedResults.NotFound(ItemNotFound(id))
            : TypedResults.Ok(item);
    }

    public static IResult Delete(int id, IInventoryService inventory) =>
        inventory.Delete(id)
            ? Results.NoContent()
            : Results.NotFound(ItemNotFound(id));

    public static IResult Head(int id, IInventoryService inventory, HttpContext context)
    {
        if (!inventory.Exists(id))
        {
            return Results.NotFound();
        }

        context.Response.Headers.ContentType = "application/json";
        context.Response.Headers.ETag = $"\"inventory-{id}\"";
        return Results.Ok();
    }

    public static IResult AllowedMethods(HttpContext context, IReadOnlyCollection<string> methods)
    {
        context.Response.Headers.Allow = string.Join(", ", methods);
        return Results.NoContent();
    }

    private static IResult? ValidateName(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? Results.BadRequest(new ErrorResponse("Name is required."))
            : null;

    private static ErrorResponse ItemNotFound(int id) =>
        new($"Inventory item {id} was not found.");
}
