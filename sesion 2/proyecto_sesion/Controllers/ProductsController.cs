using CodeSprint.Session02.Api.Models;
using CodeSprint.Session02.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeSprint.Session02.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    // ASP.NET Core entrega el servicio mediante el constructor.
    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<Product>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<Product>> GetAll()
    {
        return Ok(_productService.GetAll());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        Product? product = _productService.GetById(id);

        if (product is null)
        {
            return NotFound(new ErrorResponse { Message = "Producto no encontrado." });
        }

        return Ok(product);
    }
}
