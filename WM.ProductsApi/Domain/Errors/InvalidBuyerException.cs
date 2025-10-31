namespace WM.ProductsApi.Domain.Errors;

public sealed class InvalidBuyerException : Exception
{
    public InvalidBuyerException(string buyerId)
        : base($"Buyer '{buyerId}' is not a valid buyer.") { }
}
