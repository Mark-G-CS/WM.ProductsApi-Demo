using WM.ProductsApi.Contracts;
using WM.ProductsApi.Models;

namespace WM.ProductsApi.Contracts.Mapping;

public static class ProductMapping
{
    public static Product ToDomain(this ProductCreateDto dto) => new()
    {
        SKU = dto.SKU.Trim(),
        Title = dto.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description,
        BuyerId = dto.BuyerId.Trim(),
        Active = dto.Active
    };

    public static Product ToDomain(this ProductUpdateDto dto, string sku) => new()
    {
        SKU = sku,
        Title = dto.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description,
        BuyerId = dto.BuyerId.Trim(),
        Active = dto.Active
    };

    public static ProductReadDto ToReadDto(this Product p) => new()
    {
        SKU = p.SKU,
        Title = p.Title,
        Description = p.Description,
        BuyerId = p.BuyerId,
        Active = p.Active
    };
}
