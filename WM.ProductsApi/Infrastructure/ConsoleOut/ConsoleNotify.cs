using WM.ProductsApi.Domain.Abstractions;

namespace WM.ProductsApi.Infrastructure.ConsoleOut;

public sealed class ConsoleNotify : INotify
{
    public void Send(string message) => Console.WriteLine(message);
}
