using WM.ProductsApi.Domain;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Application.Services;

public interface IProductService
{
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
    Task<Product> UpdateAsync(string sku, Product updated, CancellationToken ct = default);
    Task<Product?> GetAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> ListAsync(ProductQuery? query = null, CancellationToken ct = default);
    Task DeleteAsync(string sku, CancellationToken ct = default);
}
