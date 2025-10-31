namespace WM.ProductsApi.Domain.Abstractions;

public interface IDeactivationNotifier
{
    Task NotifyDeactivatedAsync(string sku, string buyerId, CancellationToken ct = default);
}
