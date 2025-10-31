using WM.ProductsApi.Domain.Abstractions;

namespace WM.ProductsApi.Infrastructure.Adapters;

public sealed class NotifyAdapterDeactivationNotifier : IDeactivationNotifier
{
    private readonly INotify _notify;

    public NotifyAdapterDeactivationNotifier(INotify notify) => _notify = notify;

    public Task NotifyDeactivatedAsync(string sku, string buyerId, CancellationToken ct = default)
    {
        _notify.Send($"Notifying buyer {buyerId}: Product deactivated (SKU {sku})");
        return Task.CompletedTask;
    }
}
