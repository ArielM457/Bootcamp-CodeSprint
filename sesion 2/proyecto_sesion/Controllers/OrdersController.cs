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


        OrderResponse order = await _orderService.ProcessAsync(product, request.Quantity);

        return StatusCode(StatusCodes.Status201Created, order);
    }
}
