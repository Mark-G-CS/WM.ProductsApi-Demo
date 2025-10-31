using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Infrastructure.Decorators;

public sealed class PersistentProductRepository : IProductRepository
{
    private readonly IProductRepository _inner;
    private readonly IProductSnapshotStore _store;

    public PersistentProductRepository(IProductRepository inner, IProductSnapshotStore store)
    {
        _inner = inner;
        _store = store;

        // Hydrate synchronously at startup (singleton lifetime); acceptable for this assessment
        var snapshot = _store.LoadAsync().GetAwaiter().GetResult();
        foreach (var p in snapshot)
        {
            try { _inner.AddAsync(p).GetAwaiter().GetResult(); }
            catch { /* ignore duplicates on hydrate */ }
        }
    }

    public Task<bool> ExistsSkuAsync(string sku, CancellationToken ct = default)
        => _inner.ExistsSkuAsync(sku, ct);

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
        => _inner.GetBySkuAsync(sku, ct);

    public Task<IReadOnlyList<Product>> ListAsync(ProductQuery? query = null, CancellationToken ct = default)
        => _inner.ListAsync(query, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _inner.AddAsync(product, ct);
        await SnapshotAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(product, ct);
        await SnapshotAsync(ct);
    }

    public async Task DeleteAsync(string sku, CancellationToken ct = default)
    {
        await _inner.DeleteAsync(sku, ct);
        await SnapshotAsync(ct);
    }

    private async Task SnapshotAsync(CancellationToken ct)
    {
        var all = await _inner.ListAsync(null, ct);
        await _store.SaveAsync(all, ct);
    }
}
