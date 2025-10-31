using WM.ProductsApi.Models;

namespace WM.ProductsApi.Domain.Abstractions;

public interface IBuyerCatalog
{
    Task<bool> IsValidAsync(string buyerId, CancellationToken ct = default);
    Task<IReadOnlyList<Buyer>> ListAsync(CancellationToken ct = default);
}
