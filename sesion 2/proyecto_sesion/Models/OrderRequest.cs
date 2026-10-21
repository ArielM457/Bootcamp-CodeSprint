namespace CodeSprint.Session02.Api.Models;

// Este modelo representa los datos que envía el cliente para comprar.
public sealed class OrderRequest
{
    public int ProductId { get; init; }

    public int Quantity { get; init; }
}
