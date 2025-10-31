using WM.ProductsApi.Application.Validation;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Domain.Errors;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Application;

public sealed class ProductRules : IProductRules
{
    private readonly IBuyerCatalog _buyers;
    private readonly IProductRepository _repo;

    public ProductRules(IBuyerCatalog buyers, IProductRepository repo)
    {
        _buyers = buyers;
        _repo = repo;
    }

    public async Task ValidateOnCreateAsync(Product product, CancellationToken ct = default)
    {
        if (await _repo.ExistsSkuAsync(product.SKU, ct))
            throw new DuplicateSkuException(product.SKU);

        if (!await _buyers.IsValidAsync(product.BuyerId, ct))
            throw new InvalidBuyerException(product.BuyerId);
    }

    public async Task ValidateOnUpdateAsync(Product product, CancellationToken ct = default)
    {
        if (!await _buyers.IsValidAsync(product.BuyerId, ct))
            throw new InvalidBuyerException(product.BuyerId);
    }
}
