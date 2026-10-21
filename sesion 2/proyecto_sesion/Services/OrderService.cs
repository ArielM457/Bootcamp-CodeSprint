using CodeSprint.Session02.Api.Models;

namespace CodeSprint.Session02.Api.Services;

public sealed class OrderService
{
    public async Task<OrderResponse> ProcessAsync(Product product, int quantity)
    {
        // Task.Delay solo simula el tiempo de procesamiento.
        // No representa una operación real de entrada o salida.
        await Task.Delay(800);

        return new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            Total = product.Price * quantity,
            Status = "Compra simulada correctamente"
        };
    }
}
