namespace WM.ProductsApi.Domain;

// Optional filters for listing
public sealed record ProductQuery(bool? Active = null, string? BuyerId = null, string? Search = null);
