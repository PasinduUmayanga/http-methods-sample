using Diagnostics.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapDiagnosticsEndpoints();

app.Run();

public sealed class DiagnosticsApiMarker;
