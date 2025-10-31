namespace WM.ProductsApi.Domain.Abstractions;

// "Doc-style" external notifier (name intentionally matches doc wording)
public interface INotify
{
    void Send(string message);
}
