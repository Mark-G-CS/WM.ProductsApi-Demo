using System.ComponentModel.DataAnnotations;

namespace WM.ProductsApi.Models;

public class Product
{
    // Required, must be unique
    [Required]
    public string SKU { get; set; } = default!;

    // Required
    [Required]
    public string Title { get; set; } = default!;

    // Optional
    public string? Description { get; set; }

    // Required, must match one of the provided buyers
    [Required]
    public string BuyerId { get; set; } = default!;

    // Required: indicates if the product can be purchased
    public bool Active { get; set; } = true;
}
