using WM.ProductsApi.Application.Validation;
using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IProductRules _rules;
    private readonly IDeactivationNotifier _notifier;

    public ProductService(IProductRepository repo, IProductRules rules, IDeactivationNotifier notifier)
    {
        _repo = repo;
        _rules = rules;
        _notifier = notifier;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        await _rules.ValidateOnCreateAsync(product, ct);
        await _repo.AddAsync(product, ct);
        return (await _repo.GetBySkuAsync(product.SKU, ct))!;
    }

    public async Task<Product> UpdateAsync(string sku, Product updated,  CancellationToken ct = default)
    {
        var existing = await _repo.GetBySkuAsync(sku, ct)
            ?? throw new KeyNotFoundException($"Product with SKU '{sku}' not found.");

        var wasActive = existing.Active;

        // Apply changes
        existing.Title = updated.Title;
        existing.Description = updated.Description;
        existing.BuyerId = updated.BuyerId;
        existing.Active = updated.Active;

        await _rules.ValidateOnUpdateAsync(existing, ct);
        await _repo.UpdateAsync(existing, ct);

        if (wasActive && !existing.Active)
            await _notifier.NotifyDeactivatedAsync(existing.SKU, existing.BuyerId, ct);

        return (await _repo.GetBySkuAsync(sku, ct))!;
    }

    public Task<Product?> GetAsync(string sku, CancellationToken ct = default)
        => _repo.GetBySkuAsync(sku, ct);

    public Task<IReadOnlyList<Product>> ListAsync(ProductQuery? query = null, CancellationToken ct = default)
        => _repo.ListAsync(query, ct);

    public Task DeleteAsync(string sku, CancellationToken ct = default)
        => _repo.DeleteAsync(sku, ct);
}
