using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WM.ProductsApi.Application;
using WM.ProductsApi.Application.Validation;
using WM.ProductsApi.Domain.Abstractions;
using WM.ProductsApi.Domain.Errors;
using WM.ProductsApi.Models;
using Xunit;

namespace WM.ProductsApi.Tests.Application;

public class ProductRulesTests
{
    private static Product NewProduct(string sku = "SKU-1", string buyerId = "buyer-ok") => new()
    {
        SKU = sku,
        Title = "Test",
        Description = "Desc",
        BuyerId = buyerId,
        Active = true
    };

    [Fact]
    public async Task ValidateOnCreateAsync_Throws_WhenSkuExists()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.ExistsSkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var buyers = new Mock<IBuyerCatalog>();
        buyers.Setup(b => b.IsValidAsync("buyer-ok", It.IsAny<CancellationToken>()))
              .ReturnsAsync(true);

        IProductRules rules = new ProductRules(buyers.Object, repo.Object);

        // Act
        var act = () => rules.ValidateOnCreateAsync(NewProduct(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateSkuException>()
            .WithMessage("*SKU 'SKU-1'*");
    }

    [Fact]
    public async Task ValidateOnCreateAsync_Throws_WhenBuyerInvalid()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.ExistsSkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var buyers = new Mock<IBuyerCatalog>();
        buyers.Setup(b => b.IsValidAsync("wrong", It.IsAny<CancellationToken>()))
              .ReturnsAsync(false);

        IProductRules rules = new ProductRules(buyers.Object, repo.Object);

        var act = () => rules.ValidateOnCreateAsync(NewProduct(buyerId: "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidBuyerException>()
            .WithMessage("*Buyer 'wrong'*");
    }

    [Fact]
    public async Task ValidateOnCreateAsync_Passes_WhenValid()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.ExistsSkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var buyers = new Mock<IBuyerCatalog>();
        buyers.Setup(b => b.IsValidAsync("buyer-ok", It.IsAny<CancellationToken>()))
              .ReturnsAsync(true);

        IProductRules rules = new ProductRules(buyers.Object, repo.Object);

        await rules.Invoking(r => r.ValidateOnCreateAsync(NewProduct(), CancellationToken.None))
                   .Should().NotThrowAsync();
    }
}
