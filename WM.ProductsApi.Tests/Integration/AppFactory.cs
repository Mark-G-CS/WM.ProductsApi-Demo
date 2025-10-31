using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WM.ProductsApi.Domain.Abstractions;

namespace WM.ProductsApi.Tests.Integration;

public sealed class DeactivationSpy : IDeactivationNotifier
{
    private int _count;                     
    private readonly object _gate = new();  

    public int Count => Volatile.Read(ref _count);
    public string? LastSku { get; private set; }
    public string? LastBuyerId { get; private set; }

    public Task NotifyDeactivatedAsync(string sku, string buyerId, CancellationToken ct = default)
    {
        Interlocked.Increment(ref _count);  

        lock (_gate)
        {
            LastSku = sku;
            LastBuyerId = buyerId;
        }

        return Task.CompletedTask;
    }
}


public sealed class AppFactory : WebApplicationFactory<Program>
{
    public readonly DeactivationSpy Spy = new();
    private string? _tempFile;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _tempFile = Path.GetTempFileName();

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Persistence:FilePath"] = _tempFile // avoid touching real disk
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace any registered IDeactivationNotifier with our spy
            for (int i = services.Count - 1; i >= 0; i--)
                if (services[i].ServiceType == typeof(IDeactivationNotifier))
                    services.RemoveAt(i);

            services.AddSingleton<IDeactivationNotifier>(Spy);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && _tempFile is not null)
        {
            try { if (File.Exists(_tempFile)) File.Delete(_tempFile); } catch { /* ignore */ }
        }
    }
}
