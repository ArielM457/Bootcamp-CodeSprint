using CodeSprint.Session02.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Registramos los controladores y los servicios que usará la API.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI describe los endpoints y Swagger UI permite probarlos en el navegador.
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.MapControllers();

app.Run();
