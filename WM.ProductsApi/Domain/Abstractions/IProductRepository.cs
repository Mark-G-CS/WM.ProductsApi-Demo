using WM.ProductsApi.Models;

namespace WM.ProductsApi.Domain.Abstractions;

public interface IProductRepository
{
    Task<bool> ExistsSkuAsync(string sku, CancellationToken ct = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> ListAsync(ProductQuery? query = null, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);      // throws DuplicateSkuException
    Task UpdateAsync(Product product, CancellationToken ct = default);   // throws KeyNotFoundException
    Task DeleteAsync(string sku, CancellationToken ct = default);        // no-op if missing
}
