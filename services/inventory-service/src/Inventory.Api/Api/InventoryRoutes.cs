namespace Inventory.Api.Api;

/// <summary>
/// Centralizes Inventory API route templates and Allow-header method lists.
/// </summary>
internal static class InventoryRoutes
{
    public const string Collection = "/inventory";
    public const string ItemById = "/{id:int}";

    public static readonly string[] CollectionMethods = ["GET", "POST", "OPTIONS"];
    public static readonly string[] ItemMethods = ["GET", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];
}
