# HTTP Methods Sample

[![Build status](https://ci.appveyor.com/api/projects/status/fcrmhfsts7e1wkcr?svg=true)](https://ci.appveyor.com/project/Mahadenamuththa/http-methods-sample)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Minimal%20API-512BD4)
![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)
![Tests](https://img.shields.io/badge/tests-xUnit-5E2D91)
![Platform](https://img.shields.io/badge/platform-cross--platform-blue)

This repository contains a .NET 10 LTS microservices sample that demonstrates common and less common HTTP methods with small ASP.NET Core Minimal APIs.

## Services

| Service | Purpose | Default local command |
| --- | --- | --- |
| `Inventory.Api` | CRUD-style examples for `GET`, `POST`, `PUT`, `PATCH`, `DELETE`, `HEAD`, and `OPTIONS`. | `dotnet run --project src/Inventory.Api` |
| `Diagnostics.Api` | Protocol examples for `OPTIONS`, `TRACE`, and a safe `CONNECT` simulation. | `dotnet run --project src/Diagnostics.Api` |

## Requirements

- .NET 10 SDK

Check your SDK with:

```bash
dotnet --info
```

## Build and test

```bash
dotnet restore HttpMethodsSample.slnx
dotnet build HttpMethodsSample.slnx --configuration Release --no-restore
dotnet test tests/HttpMethodsSample.Tests/HttpMethodsSample.Tests.csproj --configuration Release --no-restore
```

`Directory.Build.props` disables NuGet audit network calls so the sample builds cleanly in offline or restricted CI environments.

## Run the sample APIs

Start the Inventory API in one terminal:

```bash
dotnet run --project src/Inventory.Api --urls http://localhost:5001
```

Start the Diagnostics API in another terminal:

```bash
dotnet run --project src/Diagnostics.Api --urls http://localhost:5002
```

## How endpoint classes are used

The APIs keep endpoint mapping code in static extension classes. `Program.cs` stays small and calls those extension methods to register routes.

Inventory API startup:

```csharp
using Inventory.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInventorySample();

var app = builder.Build();

app.MapInventoryEndpoints();

app.Run();
```

The `InventoryEndpoints` class exposes the methods used by `Program.cs`:

```csharp
public static class InventoryEndpoints
{
    public static IServiceCollection AddInventorySample(this IServiceCollection services)
    {
        services.AddSingleton<InventoryStore>();
        return services;
    }

    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/inventory", (InventoryStore store) =>
            Results.Ok(store.All()));

        endpoints.MapPost("/inventory", (CreateInventoryItemRequest request, InventoryStore store) =>
        {
            var item = store.Create(request.Name, request.Quantity);
            return Results.Created($"/inventory/{item.Id}", item);
        });

        return endpoints;
    }
}
```

Diagnostics API startup:

```csharp
using Diagnostics.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapDiagnosticsEndpoints();

app.Run();
```

The `DiagnosticsEndpoints` class follows the same pattern:

```csharp
public static class DiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/diagnostics", () =>
            Results.Ok(new { message = "Diagnostics API" }));

        endpoints.MapMethods("/diagnostics/trace", ["TRACE"], (HttpContext context) =>
            Results.Ok(new { context.Request.Method, context.Request.Path }));

        return endpoints;
    }
}
```

## HTTP method examples

### GET

Use `GET` when you want to read a resource without changing server state. It is the normal method for fetching lists, details pages, and lookup data.

Sample endpoints:

- `GET /inventory`
- `GET /inventory/{id}`

How to call:

```bash
curl http://localhost:5001/inventory
curl http://localhost:5001/inventory/1
```

How it is implemented:

```csharp
endpoints.MapGet(InventoryRoute, (InventoryStore store) =>
    Results.Ok(store.All()));

endpoints.MapGet(InventoryItemRoute, (int id, InventoryStore store) =>
{
    var item = store.Find(id);
    return item is null
        ? Results.NotFound()
        : Results.Ok(item);
});
```

### POST

Use `POST` when you want the server to create a new resource or process a command where the server decides the result URI or action.

Sample endpoint:

- `POST /inventory`

How to call:

```bash
curl -i -X POST http://localhost:5001/inventory \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Pencil\",\"quantity\":30}"
```

How it is implemented:

```csharp
endpoints.MapPost(InventoryRoute, (CreateInventoryItemRequest request, InventoryStore store, HttpContext context) =>
{
    var item = store.Create(request.Name.Trim(), request.Quantity);
    var location = $"{context.Request.Scheme}://{context.Request.Host}/inventory/{item.Id}";

    return Results.Created(location, item);
});
```

### PUT

Use `PUT` when you want to fully replace a resource at a known URI. In this sample it also works as an upsert, meaning the item is created if it does not already exist.

Sample endpoint:

- `PUT /inventory/{id}`

How to call:

```bash
curl -i -X PUT http://localhost:5001/inventory/1 \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Marker\",\"quantity\":7}"
```

How it is implemented:

```csharp
endpoints.MapPut(InventoryItemRoute, (int id, CreateInventoryItemRequest request, InventoryStore store) =>
{
    var item = store.Replace(id, request.Name.Trim(), request.Quantity);

    return Results.Ok(item);
});
```

### PATCH

Use `PATCH` when you want to update only part of a resource without sending the full replacement representation.

Sample endpoint:

- `PATCH /inventory/{id}`

How to call:

```bash
curl -i -X PATCH http://localhost:5001/inventory/1 \
  -H "Content-Type: application/json" \
  -d "{\"quantity\":25}"
```

How it is implemented:

```csharp
endpoints.MapPatch(InventoryItemRoute, (int id, PatchInventoryItemRequest request, InventoryStore store) =>
{
    var item = store.Patch(id, request.Name?.Trim(), request.Quantity);

    return item is null
        ? Results.NotFound()
        : Results.Ok(item);
});
```

### DELETE

Use `DELETE` when you want to remove a resource identified by its URI.

Sample endpoint:

- `DELETE /inventory/{id}`

How to call:

```bash
curl -i -X DELETE http://localhost:5001/inventory/1
```

How it is implemented:

```csharp
endpoints.MapDelete(InventoryItemRoute, (int id, InventoryStore store) =>
    store.Delete(id)
        ? Results.NoContent()
        : Results.NotFound());
```

### HEAD

Use `HEAD` when you need the same headers you would get from `GET`, but without downloading the response body. It is useful for metadata checks, cache validation, and lightweight existence checks.

Sample endpoint:

- `HEAD /inventory/{id}`

How to call:

```bash
curl -I http://localhost:5001/inventory/1
```

How it is implemented:

```csharp
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
```

### OPTIONS

Use `OPTIONS` when you want to discover which HTTP methods an endpoint supports. APIs often return an `Allow` header in the response.

Sample endpoints:

- `OPTIONS /inventory`
- `OPTIONS /inventory/{id}`
- `OPTIONS /diagnostics`

How to call:

```bash
curl -i -X OPTIONS http://localhost:5001/inventory
curl -i -X OPTIONS http://localhost:5001/inventory/1
curl -i -X OPTIONS http://localhost:5002/diagnostics
```

How it is implemented:

```csharp
endpoints.MapMethods(InventoryRoute, ["OPTIONS"], () =>
    Results.NoContent().WithHeader("Allow", "GET, POST, OPTIONS"));

endpoints.MapMethods(InventoryItemRoute, ["OPTIONS"], () =>
    Results.NoContent().WithHeader("Allow", "GET, PUT, PATCH, DELETE, HEAD, OPTIONS"));

endpoints.MapMethods("/diagnostics", ["OPTIONS"], () =>
    Results.NoContent().WithHeader("Allow", "GET, OPTIONS, TRACE, CONNECT"));
```

### TRACE

Use `TRACE` when diagnosing request routing because it echoes request metadata back to the caller. Many production systems disable it for security reasons. This sample redacts sensitive headers such as `Authorization`, `Cookie`, and `Proxy-Authorization`.

Sample endpoint:

- `TRACE /diagnostics/trace`

How to call:

```bash
curl -i -X TRACE "http://localhost:5002/diagnostics/trace?demo=true" \
  -H "Authorization: Bearer secret" \
  -H "X-Demo: visible"
```

How it is implemented:

```csharp
endpoints.MapMethods("/diagnostics/trace", ["TRACE"], (HttpContext context) =>
{
    var headers = context.Request.Headers.ToDictionary(
        header => header.Key,
        header => SensitiveHeaders.Contains(header.Key) ? "[redacted]" : header.Value.ToString());

    return Results.Ok(new TraceEcho(
        context.Request.Method,
        context.Request.Scheme,
        context.Request.Host.ToString(),
        context.Request.Path,
        context.Request.QueryString.ToString(),
        headers));
});
```

### CONNECT

Use `CONNECT` when an HTTP proxy needs to open a tunnel to another server, commonly for HTTPS proxying. Normal application APIs rarely use it directly. This project demonstrates the method safely: the endpoint acknowledges the request but does not create a real tunnel.

Sample endpoint:

- `CONNECT /diagnostics/connect/{authority}`

How to call the real method:

```bash
curl -i -X CONNECT http://localhost:5002/diagnostics/connect/example.com:443
```

Some local clients and servers apply special `CONNECT` handling before normal routing. For day-to-day testing and CI, the sample exposes the same safe demo response through `POST` while still mapping the real `CONNECT` method.

CI-friendly mirror call:

```bash
curl -i -X POST http://localhost:5002/diagnostics/connect/example.com:443
```

How it is implemented:

```csharp
endpoints.MapMethods("/diagnostics/connect/{authority}", ["CONNECT", "POST"], (string authority) =>
    Results.Ok(new ConnectDemo(
        authority,
        "CONNECT normally asks an HTTP proxy to open a tunnel. This sample acknowledges the request but does not create a network tunnel.",
        false)));
```

## Method summary

| Method | Typical use | Sample endpoint |
| --- | --- | --- |
| `GET` | Read resources. | `GET /inventory`, `GET /inventory/{id}` |
| `POST` | Create a resource or submit a command. | `POST /inventory` |
| `PUT` | Replace or upsert a resource. | `PUT /inventory/{id}` |
| `PATCH` | Partially update a resource. | `PATCH /inventory/{id}` |
| `DELETE` | Remove a resource. | `DELETE /inventory/{id}` |
| `HEAD` | Read response headers without a body. | `HEAD /inventory/{id}` |
| `OPTIONS` | Discover allowed methods. | `OPTIONS /inventory`, `OPTIONS /diagnostics` |
| `TRACE` | Echo request metadata for diagnostics. | `TRACE /diagnostics/trace` |
| `CONNECT` | Request a proxy tunnel. | `CONNECT /diagnostics/connect/{authority}` |
