# Agent Guide

## Project purpose

This repository is an educational .NET 10 LTS sample for demonstrating HTTP methods through small ASP.NET Core Minimal API microservices.

## Layout

- `src/Inventory.Api`: CRUD-oriented HTTP method examples.
- `src/Diagnostics.Api`: protocol-oriented method examples for `OPTIONS`, `TRACE`, and `CONNECT`.
- `tests/HttpMethodsSample.Tests`: xUnit integration tests that start the APIs on temporary local ports.
- `Directory.Build.props`: shared MSBuild settings, including disabled NuGet audit network calls for deterministic restricted builds.

## Commands

```bash
dotnet restore HttpMethodsSample.slnx
dotnet build HttpMethodsSample.slnx --configuration Release --no-restore
dotnet test tests/HttpMethodsSample.Tests/HttpMethodsSample.Tests.csproj --configuration Release --no-restore
```

Run services locally:

```bash
dotnet run --project src/Inventory.Api --urls http://localhost:5001
dotnet run --project src/Diagnostics.Api --urls http://localhost:5002
```

## Conventions

- Keep endpoint registration in dedicated extension methods so apps and tests use the same mappings.
- Keep data in-memory; this project is a protocol demonstration, not a persistence sample.
- Keep `TRACE` sanitized and redact sensitive headers.
- Keep `CONNECT` safe: demonstrate the method without opening real tunnels.
- Add or update tests whenever changing endpoint behavior.
