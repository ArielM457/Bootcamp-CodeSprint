namespace CodeSprint.Session02.Api.Models;

// Este modelo representa la respuesta de una compra simulada.
public sealed class OrderResponse
{
    public Guid OrderId { get; init; }

    public int ProductId { get; init; }

    public required string ProductName { get; init; }

    public int Quantity { get; init; }

    public decimal Total { get; init; }

    public required string Status { get; init; }
}
