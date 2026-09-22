# Agent Guide

## Project purpose

This repository is an educational .NET 10 LTS sample for demonstrating HTTP methods through small ASP.NET Core Minimal API microservices.

## Layout

- `services/inventory-service/src`: Inventory onion-architecture projects.
- `services/inventory-service/tests`: Inventory API integration tests.
- `services/diagnostics-service/src`: Diagnostics onion-architecture projects.
- `services/diagnostics-service/tests`: Diagnostics API integration tests.
- `Directory.Build.props`: shared MSBuild settings, including disabled NuGet audit network calls for deterministic restricted builds.

## Commands

```bash
dotnet restore HttpMethodsSample.slnx
dotnet build HttpMethodsSample.slnx --configuration Release --no-restore
dotnet test services/inventory-service/tests/Inventory.Tests/Inventory.Tests.csproj --configuration Release --no-restore
dotnet test services/diagnostics-service/tests/Diagnostics.Tests/Diagnostics.Tests.csproj --configuration Release --no-restore
```

Run services locally:

```bash
dotnet run --project services/inventory-service/src/Inventory.Api --urls http://localhost:5001
dotnet run --project services/diagnostics-service/src/Diagnostics.Api --urls http://localhost:5002
```

## Conventions

- Keep endpoint registration in dedicated extension methods so apps and tests use the same mappings.
- Keep data in-memory; this project is a protocol demonstration, not a persistence sample.
- Keep `TRACE` sanitized and redact sensitive headers.
- Keep `CONNECT` safe: demonstrate the method without opening real tunnels.
- Add or update tests whenever changing endpoint behavior.
