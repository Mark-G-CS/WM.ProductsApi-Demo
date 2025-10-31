using WM.ProductsApi.Data;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Infrastructure.InMemory;

public sealed class InMemoryBuyerCatalog : IBuyerCatalog
{
    public Task<bool> IsValidAsync(string buyerId, CancellationToken ct = default)
        => Task.FromResult(SeedData.Buyers.Any(b => b.Id == buyerId));

    public Task<IReadOnlyList<Buyer>> ListAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Buyer>)SeedData.Buyers.ToList());
}
