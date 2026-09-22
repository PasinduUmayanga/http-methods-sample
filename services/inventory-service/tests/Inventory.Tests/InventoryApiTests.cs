using System.Net;
using System.Net.Http.Json;
using Inventory.Api.Api;
using Inventory.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Tests;

public sealed class InventoryApiTests
{
    [Fact]
    public async Task GetCollectionListsSeedInventoryItems()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var items = await app.Client.GetFromJsonAsync<InventoryItem[]>("/inventory");

        Assert.NotNull(items);
        Assert.Contains(items, item => item.Id == 1 && item.Name == "Sample notebook");
    }

    [Fact]
    public async Task GetItemReturnsInventoryItemById()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var item = await app.Client.GetFromJsonAsync<InventoryItem>("/inventory/1");

        Assert.Equal(new InventoryItem(1, "Sample notebook", 12), item);
    }

    [Fact]
    public async Task PostCollectionCreatesInventoryItem()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var response = await app.Client.PostAsJsonAsync("/inventory", new CreateInventoryItemRequest("Pencil", 30));
        var item = await response.Content.ReadFromJsonAsync<InventoryItem>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(item);
        Assert.Equal("Pencil", item.Name);
        Assert.Equal(30, item.Quantity);
    }

    [Fact]
    public async Task PutItemReplacesInventoryItem()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var response = await app.Client.PutAsJsonAsync("/inventory/1", new CreateInventoryItemRequest("Marker", 7));
        var item = await response.Content.ReadFromJsonAsync<InventoryItem>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new InventoryItem(1, "Marker", 7), item);
    }

    [Fact]
    public async Task PatchItemPartiallyUpdatesInventoryItem()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var response = await app.Client.PatchAsJsonAsync("/inventory/1", new PatchInventoryItemRequest(null, 25));
        var item = await response.Content.ReadFromJsonAsync<InventoryItem>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Sample notebook", item?.Name);
        Assert.Equal(25, item?.Quantity);
    }

    [Fact]
    public async Task DeleteItemRemovesInventoryItem()
    {
        await using var app = await TestApplication.StartInventoryAsync();

        var deleteResponse = await app.Client.DeleteAsync("/inventory/1");
        var getResponse = await app.Client.GetAsync("/inventory/1");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task HeadItemReturnsHeadersWithoutBody()
    {
        await using var app = await TestApplication.StartInventoryAsync();
        using var request = new HttpRequestMessage(HttpMethod.Head, "/inventory/1");

        var response = await app.Client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(response.Headers.ETag?.Tag, new[] { "\"inventory-1\"" });
        Assert.Empty(body);
    }

    [Fact]
    public async Task OptionsCollectionReturnsAllowedCollectionMethods()
    {
        await using var app = await TestApplication.StartInventoryAsync();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/inventory");

        var response = await app.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("GET, POST, OPTIONS", response.Content.Headers.Allow.ToString());
    }

    [Fact]
    public async Task OptionsItemReturnsAllowedItemMethods()
    {
        await using var app = await TestApplication.StartInventoryAsync();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/inventory/1");

        var response = await app.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("GET, PUT, PATCH, DELETE, HEAD, OPTIONS", response.Content.Headers.Allow.ToString());
    }
}

internal static class TestApplication
{
    public static async Task<RunningApplication> StartInventoryAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddInventorySample();

        var app = builder.Build();
        app.MapInventoryEndpoints();

        await app.StartAsync();

        return RunningApplication.From(app);
    }
}

internal sealed class RunningApplication(WebApplication app, HttpClient client) : IAsyncDisposable
{
    public HttpClient Client { get; } = client;

    public EndpointDataSource Endpoints { get; } = app.Services.GetRequiredService<EndpointDataSource>();

    public static RunningApplication From(WebApplication app)
    {
        var address = app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()?
            .Addresses
            .Single();

        if (address is null)
        {
            throw new InvalidOperationException("The test server did not publish a listening address.");
        }

        return new RunningApplication(app, new HttpClient { BaseAddress = new Uri(address) });
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await app.DisposeAsync();
    }
}
