using Microsoft.AspNetCore.Mvc;
using WM.ProductsApi.Application.Services;
using WM.ProductsApi.Contracts;
using WM.ProductsApi.Contracts.Mapping;
using WM.ProductsApi.Domain;
using WM.ProductsApi.Domain.Errors;

namespace WM.ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _svc;

    public ProductsController(IProductService svc) => _svc = svc;

    // GET /api/products?active=true&buyerId=...&search=abc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> List([FromQuery] bool? active, [FromQuery] string? buyerId, [FromQuery] string? search, CancellationToken ct)
    {
        var q = new ProductQuery(active, buyerId, search);
        var items = await _svc.ListAsync(q, ct);
        return Ok(items.Select(p => p.ToReadDto()));
    }

    // GET /api/products/{sku}
    [HttpGet("{sku}")]
    public async Task<ActionResult<ProductReadDto>> Get(string sku, CancellationToken ct)
    {
        var p = await _svc.GetAsync(sku, ct);
        return p is null ? NotFound() : Ok(p.ToReadDto());
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _svc.CreateAsync(dto.ToDomain(), ct);
            return CreatedAtAction(nameof(Get), new { sku = created.SKU }, created.ToReadDto());
        }
        catch (DuplicateSkuException ex)
        {
            return Conflict(Problem(title: "Duplicate SKU", detail: ex.Message, statusCode: StatusCodes.Status409Conflict));
        }
        catch (InvalidBuyerException ex)
        {
            return BadRequest(Problem(title: "Invalid Buyer", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest));
        }
    }

    // PUT /api/products/{sku}
    [HttpPut("{sku}")]
    public async Task<ActionResult<ProductReadDto>> Update(string sku, [FromBody] ProductUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _svc.UpdateAsync(sku, dto.ToDomain(sku), ct);
            return Ok(updated.ToReadDto());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidBuyerException ex)
        {
            return BadRequest(Problem(title: "Invalid Buyer", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest));
        }
    }

    // DELETE /api/products/{sku}
    [HttpDelete("{sku}")]
    public async Task<IActionResult> Delete(string sku, CancellationToken ct)
    {
        await _svc.DeleteAsync(sku, ct);
        return NoContent(); // idempotent
    }
}
