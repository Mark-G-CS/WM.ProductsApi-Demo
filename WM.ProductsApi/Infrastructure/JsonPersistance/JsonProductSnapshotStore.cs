using System.Text.Json;
using Microsoft.Extensions.Options;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Infrastructure.JsonPersistence;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Infrastructure.JsonPersistence;

public sealed class JsonProductSnapshotStore : IProductSnapshotStore
{
    private readonly string _absolutePath;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public JsonProductSnapshotStore(IOptions<JsonPersistenceOptions> opt, IWebHostEnvironment env)
    {
        var path = opt.Value.FilePath ?? "AppData/products.json";
        _absolutePath = Path.IsPathRooted(path) ? path : Path.Combine(env.ContentRootPath, path);
        var dir = Path.GetDirectoryName(_absolutePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);
    }

    public async Task<IReadOnlyList<Product>> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_absolutePath)) return Array.Empty<Product>();

        var fi = new FileInfo(_absolutePath);
        if (fi.Length == 0) return Array.Empty<Product>(); // <-- handle empty file

        try
        {
            await using var fs = File.OpenRead(_absolutePath);
            var list = await JsonSerializer.DeserializeAsync<List<Product>>(fs, _json, ct);
            return (list ?? new()).AsReadOnly();
        }
        catch (JsonException)
        {
            // Corrupt or partial file? Start clean instead of crashing the app
            return Array.Empty<Product>();
        }
    }


    public async Task SaveAsync(IEnumerable<Product> products, CancellationToken ct = default)
    {
        await using var fs = File.Create(_absolutePath);
        await JsonSerializer.SerializeAsync(fs, products, _json, ct);
    }
}
