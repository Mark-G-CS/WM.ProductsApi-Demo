using WM.ProductsApi.Domain.Abstractions;

namespace WM.ProductsApi.Infrastructure.ConsoleOut;

public sealed class ConsoleDeactivationNotifier : IDeactivationNotifier
{
    public Task NotifyDeactivatedAsync(string sku, string buyerId, CancellationToken ct = default)
    {
        Console.WriteLine($"[DEACTIVATED] SKU '{sku}' at {DateTime.UtcNow:o}");
        return Task.CompletedTask;
    }
}

//SRP keeps notification separate from business logic, and DIP lets us switch to email, queue, etc.