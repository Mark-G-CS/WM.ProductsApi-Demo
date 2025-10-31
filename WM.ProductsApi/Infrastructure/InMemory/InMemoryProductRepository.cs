using System.Collections.Concurrent;
using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Domain.Errors;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Infrastructure.InMemory;

public sealed class InMemoryProductRepository : IProductRepository
{
    // SKU -> Product
    private readonly ConcurrentDictionary<string, Product> _store = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> ExistsSkuAsync(string sku, CancellationToken ct = default)
        => Task.FromResult(_store.ContainsKey(sku));

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        _store.TryGetValue(sku, out var product);
        // Return a defensive copy so callers can’t mutate internal state inadvertently
        return Task.FromResult(product is null ? null : Clone(product));
    }

    public Task<IReadOnlyList<Product>> ListAsync(ProductQuery? query = null, CancellationToken ct = default)
    {
        IEnumerable<Product> items = _store.Values.Select(Clone);

        if (query is not null)
        {
            if (query.Active is not null)
                items = items.Where(p => p.Active == query.Active.Value);

            if (!string.IsNullOrWhiteSpace(query.BuyerId))
                items = items.Where(p => p.BuyerId.Equals(query.BuyerId, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search!.Trim();
                items = items.Where(p =>
                    (p.SKU?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Title?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Description?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
            }
        }

        return Task.FromResult((IReadOnlyList<Product>)items.ToList());
    }

    public Task AddAsync(Product product, CancellationToken ct = default)
    {
        // SKU uniqueness at the persistence layer
        if (!_store.TryAdd(product.SKU, Clone(product)))
            throw new DuplicateSkuException(product.SKU);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        if (!_store.ContainsKey(product.SKU))
            throw new KeyNotFoundException($"Product with SKU '{product.SKU}' not found.");

        _store[product.SKU] = Clone(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string sku, CancellationToken ct = default)
    {
        _store.TryRemove(sku, out _);
        return Task.CompletedTask;
    }

    private static Product Clone(Product p) => new()
    {
        SKU = p.SKU,
        Title = p.Title,
        Description = p.Description,
        BuyerId = p.BuyerId,
        Active = p.Active
    };
}
