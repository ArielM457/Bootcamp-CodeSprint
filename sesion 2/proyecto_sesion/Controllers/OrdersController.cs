using CodeSprint.Session02.Api.Models;
using CodeSprint.Session02.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeSprint.Session02.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly OrderService _orderService;

    public OrdersController(ProductService productService, OrderService orderService)
    {
        _productService = productService;
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Create(OrderRequest request)
    {
        if (request.ProductId <= 0 || request.Quantity <= 0)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "El identificador y la cantidad deben ser mayores que cero."
            });
        }

        Product? product = _productService.GetById(request.ProductId);

        if (product is null)
        {
            return NotFound(new ErrorResponse { Message = "Producto no encontrado." });
        }

        if (request.Quantity > product.Stock)
        {
            return BadRequest(new ErrorResponse { Message = "No hay suficiente stock." });
        }

        OrderResponse order = await _orderService.ProcessAsync(product, request.Quantity);

        return StatusCode(StatusCodes.Status201Created, order);
    }
}
