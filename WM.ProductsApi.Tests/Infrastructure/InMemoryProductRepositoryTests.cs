using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Errors;
using WM.ProductsApi.Infrastructure.InMemory;
using WM.ProductsApi.Models;
using Xunit;

namespace WM.ProductsApi.Tests.Infrastructure;

public class InMemoryProductRepositoryTests
{
    private static Product P(string sku, bool active = true) => new()
    {
        SKU = sku,
        Title = "T",
        BuyerId = "buyer",
        Active = active
    };

    [Fact]
    public async Task AddAsync_Throws_OnDuplicateSku()
    {
        var repo = new InMemoryProductRepository();
        await repo.AddAsync(P("A"), CancellationToken.None);

        var act = () => repo.AddAsync(P("A"), CancellationToken.None);

        await act.Should().ThrowAsync<DuplicateSkuException>();
    }

    [Fact]
    public async Task ListAsync_FiltersByActive_AndSearch()
    {
        var repo = new InMemoryProductRepository();
        await repo.AddAsync(P("ON-1", true));
        await repo.AddAsync(P("OFF-1", false));

        var activeOnly = await repo.ListAsync(new ProductQuery(Active: true), CancellationToken.None);
        activeOnly.Should().HaveCount(1).And.OnlyContain(p => p.Active);

        var searchOn = await repo.ListAsync(new ProductQuery(Search: "ON"), CancellationToken.None);
        searchOn.Should().HaveCount(1);
        searchOn.Single().SKU.Should().Be("ON-1");
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenMissing()
    {
        var repo = new InMemoryProductRepository();
        var act = () => repo.UpdateAsync(P("NOPE"), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
