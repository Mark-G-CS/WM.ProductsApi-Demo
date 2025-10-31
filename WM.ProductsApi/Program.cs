using WM.ProductsApi.Application;
using WM.ProductsApi.Application.Services;
using WM.ProductsApi.Application.Validation;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Infrastructure.InMemory;        // InMemoryBuyerCatalog, InMemoryProductRepository
using WM.ProductsApi.Infrastructure.JsonPersistence; // JsonProductSnapshotStore, JsonPersistenceOptions
using WM.ProductsApi.Infrastructure.Decorators;      // PersistentProductRepository
using WM.ProductsApi.Infrastructure.ConsoleOut;       // ConsoleNotify
using WM.ProductsApi.Infrastructure.Adapters;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Core dependencies
builder.Services.AddSingleton<IBuyerCatalog, InMemoryBuyerCatalog>();   //

// JSON persistence (options + store)
builder.Services.Configure<JsonPersistenceOptions>(builder.Configuration.GetSection("Persistence"));
builder.Services.AddSingleton<IProductSnapshotStore, JsonProductSnapshotStore>();

// Repository: inner + decorator
builder.Services.AddSingleton<InMemoryProductRepository>();
builder.Services.AddSingleton<IProductRepository>(sp =>
    new PersistentProductRepository(
        sp.GetRequiredService<InMemoryProductRepository>(),
        sp.GetRequiredService<IProductSnapshotStore>()));

// Business rules + service
builder.Services.AddSingleton<IProductRules, ProductRules>();
builder.Services.AddSingleton<IProductService, ProductService>();

// Deactivation notifier via INotify adapter
builder.Services.AddSingleton<INotify, ConsoleNotify>();
builder.Services.AddSingleton<IDeactivationNotifier, NotifyAdapterDeactivationNotifier>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }
