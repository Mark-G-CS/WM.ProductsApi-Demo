using WM.ProductsApi.Models;

namespace WM.ProductsApi.Application.Validation;

public interface IProductRules
{
    Task ValidateOnCreateAsync(Product product, CancellationToken ct = default);
    Task ValidateOnUpdateAsync(Product product, CancellationToken ct = default);
}
