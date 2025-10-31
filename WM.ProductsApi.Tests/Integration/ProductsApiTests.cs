using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WM.ProductsApi.Contracts; // ProductCreateDto, ProductUpdateDto, ProductReadDto
using Xunit;

namespace WM.ProductsApi.Tests.Integration;

[CollectionDefinition("ApiCollection", DisableParallelization = true)]
public class ApiCollection : ICollectionFixture<AppFactory> { }

[Collection("ApiCollection")]
public class ProductsApiTests
{
    private readonly AppFactory _factory;
    private readonly HttpClient _client;

    private const string BuyerJohnny = "49ec2a8703224eea9dec16b22546477e"; // from SeedData
    private const string BuyerJennie = "a790a7b6bf2a48569066c46306c3332d"; // from SeedData

    public ProductsApiTests(AppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new() { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Create_Then_Get_Works()
    {
        var create = new ProductCreateDto
        {
            SKU = "IT-001",
            Title = "Item",
            Description = "Desc",
            BuyerId = BuyerJohnny,
            Active = true
        };

        var post = await _client.PostAsJsonAsync("/api/Products", create);
        post.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await _client.GetAsync("/api/Products/IT-001");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await get.Content.ReadFromJsonAsync<ProductReadDto>();
        body!.SKU.Should().Be("IT-001");
        body.BuyerId.Should().Be(BuyerJohnny);
        body.Active.Should().BeTrue();
    }

    [Fact]
    public async Task Update_Deactivate_FiresNotifier()
    {
        // Arrange: create active product
        await _client.PostAsJsonAsync("/api/Products", new ProductCreateDto
        {
            SKU = "IT-002",
            Title = "Item2",
            BuyerId = BuyerJennie,
            Active = true
        });

        // Act: deactivate
        var put = await _client.PutAsJsonAsync("/api/Products/IT-002", new ProductUpdateDto
        {
            Title = "Item2 (off)",
            Description = "now off",
            BuyerId = BuyerJennie,
            Active = false
        });

        // Assert HTTP
        put.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert side-effect via spy
        _factory.Spy.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_WithInvalidBuyer_Returns400()
    {
        var bad = new ProductCreateDto
        {
            SKU = "BAD-001",
            Title = "Bad",
            BuyerId = "not-a-valid-buyer",
            Active = true
        };

        var res = await _client.PostAsJsonAsync("/api/Products", bad);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task List_Filter_ByActive()
    {
        await _client.PostAsJsonAsync("/api/Products", new ProductCreateDto
        {
            SKU = "ON-1",
            Title = "On",
            BuyerId = BuyerJohnny,
            Active = true
        });
        await _client.PostAsJsonAsync("/api/Products", new ProductCreateDto
        {
            SKU = "OFF-1",
            Title = "Off",
            BuyerId = BuyerJohnny,
            Active = false
        });

        var res = await _client.GetAsync("/api/Products?active=false");
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await res.Content.ReadFromJsonAsync<List<ProductReadDto>>();
        items!.Should().ContainSingle(p => p.SKU == "OFF-1");
    }
}
