using Diagnostics.Api.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDiagnosticsSample();

var app = builder.Build();

app.MapDiagnosticsEndpoints();

app.Run();

public sealed class DiagnosticsApiMarker;
