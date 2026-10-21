namespace CodeSprint.Session02.Api.Models;

// Esta clase representa un producto de la tienda.
public sealed class Product
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public int Stock { get; init; }
}
