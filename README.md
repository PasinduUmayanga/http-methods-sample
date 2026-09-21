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

### DELETE

Use `DELETE` when you want to remove a resource identified by its URI.

Sample endpoint:

- `DELETE /inventory/{id}`

How to call:

```bash
curl -i -X DELETE http://localhost:5001/inventory/1
```

### HEAD

Use `HEAD` when you need the same headers you would get from `GET`, but without downloading the response body. It is useful for metadata checks, cache validation, and lightweight existence checks.

Sample endpoint:

- `HEAD /inventory/{id}`

How to call:

```bash
curl -I http://localhost:5001/inventory/1
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
