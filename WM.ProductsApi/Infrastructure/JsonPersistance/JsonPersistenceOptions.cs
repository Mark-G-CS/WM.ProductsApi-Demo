namespace WM.ProductsApi.Infrastructure.JsonPersistence;

public sealed class JsonPersistenceOptions
{
    // Relative paths resolve under the app’s ContentRoot (project folder when running from VS)
    public string FilePath { get; set; } = "AppData/products.json";
}
