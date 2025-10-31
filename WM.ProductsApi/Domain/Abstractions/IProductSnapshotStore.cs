using WM.ProductsApi.Models;

namespace WM.ProductsApi.Domain.Abstractions;

public interface IProductSnapshotStore
{
    Task<IReadOnlyList<Product>> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(IEnumerable<Product> products, CancellationToken ct = default);
}
