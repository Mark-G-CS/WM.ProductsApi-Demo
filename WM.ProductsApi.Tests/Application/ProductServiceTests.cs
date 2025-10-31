using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WM.ProductsApi.Application.Services;
using WM.ProductsApi.Application.Validation;
using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Domain.Errors;
using WM.ProductsApi.Models;
using Xunit;

namespace WM.ProductsApi.Tests.Application;

public class ProductServiceTests
{
    private static Product P(bool active = true) => new()
    {
        SKU = "SKU-1",
        Title = "T",
        Description = "D",
        BuyerId = "buyer-ok",
        Active = active
    };

    [Fact]
    public async Task CreateAsync_RunsRules_AndPersists()
    {
        var repo = new Mock<IProductRepository>();
        var rules = new Mock<IProductRules>();
        var notifier = new Mock<IDeactivationNotifier>();

        // After AddAsync, service fetches the product
        repo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repo.Setup(r => r.GetBySkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(P());

        IProductService svc = new ProductService(repo.Object, rules.Object, notifier.Object);

        var created = await svc.CreateAsync(P(), CancellationToken.None);

        rules.Verify(r => r.ValidateOnCreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        created.SKU.Should().Be("SKU-1");
    }


    [Fact]
    public async Task UpdateAsync_WhenDeactivated_FiresNotifier()
    {
        var repo = new Mock<IProductRepository>();
        var rules = new Mock<IProductRules>();
        var notifier = new Mock<IDeactivationNotifier>();

        repo.SetupSequence(r => r.GetBySkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { SKU = "SKU-1", Title = "T", Description = "D", BuyerId = "buyer-ok", Active = true }) // existing
            .ReturnsAsync(new Product { SKU = "SKU-1", Title = "T2", Description = "D2", BuyerId = "buyer-ok", Active = false }); // after update

        repo.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        IProductService svc = new ProductService(repo.Object, rules.Object, notifier.Object);

        var updated = await svc.UpdateAsync("SKU-1", new Product
        {
            SKU = "SKU-1",
            Title = "T2",
            Description = "D2",
            BuyerId = "buyer-ok",
            Active = false
        }, CancellationToken.None);

        notifier.Verify(n => n.NotifyDeactivatedAsync("SKU-1", "buyer-ok", It.IsAny<CancellationToken>()), Times.Once);
        updated.Active.Should().BeFalse();
    }


    [Fact]
    public async Task UpdateAsync_WhenNotFound_Throws()
    {
        var repo = new Mock<IProductRepository>();
        var rules = new Mock<IProductRules>();
        var notifier = new Mock<IDeactivationNotifier>();

        repo.Setup(r => r.GetBySkuAsync("MISSING", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        IProductService svc = new ProductService(repo.Object, rules.Object, notifier.Object);

        var act = () => svc.UpdateAsync("MISSING", P(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateSku_PropagatesException()
    {
        var repo = new Mock<IProductRepository>();
        var rules = new Mock<IProductRules>();
        var notifier = new Mock<IDeactivationNotifier>();

        rules.Setup(r => r.ValidateOnCreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
             .ThrowsAsync(new DuplicateSkuException("SKU-1"));

        IProductService svc = new ProductService(repo.Object, rules.Object, notifier.Object);

        var act = () => svc.CreateAsync(P(), CancellationToken.None);

        await act.Should().ThrowAsync<DuplicateSkuException>();
    }
}
