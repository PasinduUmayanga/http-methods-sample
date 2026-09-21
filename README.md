# HTTP Methods Sample

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

## Run the Inventory API

```bash
dotnet run --project src/Inventory.Api --urls http://localhost:5001
```

### GET

Use `GET` to read a collection or resource.

```bash
curl http://localhost:5001/inventory
curl http://localhost:5001/inventory/1
```

### POST

Use `POST` to create a new resource.

```bash
curl -i -X POST http://localhost:5001/inventory \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Pencil\",\"quantity\":30}"
```

### PUT

Use `PUT` to fully replace or upsert a resource at a known URI.

```bash
curl -i -X PUT http://localhost:5001/inventory/1 \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Marker\",\"quantity\":7}"
```

### PATCH

Use `PATCH` to update part of a resource.

```bash
curl -i -X PATCH http://localhost:5001/inventory/1 \
  -H "Content-Type: application/json" \
  -d "{\"quantity\":25}"
```

### DELETE

Use `DELETE` to remove a resource.

```bash
curl -i -X DELETE http://localhost:5001/inventory/1
```

### HEAD

Use `HEAD` to fetch headers for a resource without a response body.

```bash
curl -I http://localhost:5001/inventory/1
```

### OPTIONS

Use `OPTIONS` to ask an endpoint which methods it supports.

```bash
curl -i -X OPTIONS http://localhost:5001/inventory
curl -i -X OPTIONS http://localhost:5001/inventory/1
```

## Run the Diagnostics API

```bash
dotnet run --project src/Diagnostics.Api --urls http://localhost:5002
```

### OPTIONS

```bash
curl -i -X OPTIONS http://localhost:5002/diagnostics
```

### TRACE

Use `TRACE` to echo sanitized request metadata. The sample redacts sensitive headers such as `Authorization`, `Cookie`, and `Proxy-Authorization`.

```bash
curl -i -X TRACE "http://localhost:5002/diagnostics/trace?demo=true" \
  -H "Authorization: Bearer secret" \
  -H "X-Demo: visible"
```

### CONNECT

`CONNECT` normally asks an HTTP proxy to open a tunnel to another server. This project demonstrates the method safely: the endpoint acknowledges the request but does not create a tunnel.

Some local clients and servers apply special CONNECT handling before normal routing. For day-to-day testing, the sample exposes the same response through `POST` while still mapping the real `CONNECT` method.

```bash
curl -i -X CONNECT http://localhost:5002/diagnostics/connect/example.com:443
```

CI-friendly mirror:

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
