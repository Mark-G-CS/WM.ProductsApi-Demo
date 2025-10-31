using System.ComponentModel.DataAnnotations;

namespace WM.ProductsApi.Contracts;

public sealed class ProductCreateDto
{
    [Required] public string SKU { get; set; } = default!;
    [Required] public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required] public string BuyerId { get; set; } = default!;
    public bool Active { get; set; } = true;
}

public sealed class ProductUpdateDto
{
    [Required] public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required] public string BuyerId { get; set; } = default!;
    [Required] public bool Active { get; set; } = true;
}

public sealed class ProductReadDto
{
    public string SKU { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string BuyerId { get; set; } = default!;
    public bool Active { get; set; }
}
