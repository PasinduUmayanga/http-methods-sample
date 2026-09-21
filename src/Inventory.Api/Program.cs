using Inventory.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInventorySample();

var app = builder.Build();

app.MapInventoryEndpoints();

app.Run();

public sealed class InventoryApiMarker;
