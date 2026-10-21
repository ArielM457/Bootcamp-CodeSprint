using CodeSprint.Session02.Api.Models;

namespace CodeSprint.Session02.Api.Services;

public sealed class ProductService
{
    // La lista vive en memoria. Al detener la API, sus datos se pierden.
    private readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Laptop", Price = 4500.00m, Stock = 5 },
        new Product { Id = 2, Name = "Teclado mecánico", Price = 350.50m, Stock = 12 },
        new Product { Id = 3, Name = "Mouse inalámbrico", Price = 180.00m, Stock = 20 }
    ];

    public IReadOnlyList<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(product => product.Id == id);
    }
}
