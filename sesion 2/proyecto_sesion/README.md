# **Sesión 02: API de una tienda de electrónicos**

En esta sesión creamos una API pequeña con C#, .NET 10 y ASP.NET Core. La API permite consultar productos y simular una compra sin usar una base de datos.

El proyecto sirve para explicar tipos de datos, propiedades, clases, objetos, métodos, parámetros, `async/await`, controladores, endpoints, métodos HTTP y códigos de respuesta.

## **1. Requisitos**

- .NET SDK 10.
- Visual Studio Code.
- La extensión C# Dev Kit para Visual Studio Code.

Comprueba la instalación de .NET:

```powershell
dotnet --version
```

El resultado debe comenzar con `10`.

## **2. Abrir el proyecto en Visual Studio Code**

Abre PowerShell y ejecuta:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\sesiones - backend\sesion 2\proyecto_sesion"
code .
```

## **3. Estructura**

```text
proyecto_sesion/
├── Controllers/
│   ├── HealthController.cs
│   ├── OrdersController.cs
│   └── ProductsController.cs
├── Models/
│   ├── ErrorResponse.cs
│   ├── HealthResponse.cs
│   ├── OrderRequest.cs
│   ├── OrderResponse.cs
│   └── Product.cs
├── Services/
│   ├── OrderService.cs
│   └── ProductService.cs
├── Properties/
│   └── launchSettings.json
├── CodeSprint.Session02.Api.csproj
├── CodeSprint.Session02.Api.http
├── Program.cs
└── README.md
```

- `Models` contiene las clases que representan los datos.
- `Services` contiene la lista en memoria y el procesamiento de la compra.
- `Controllers` recibe las peticiones HTTP y devuelve respuestas.
- `Program.cs` configura y arranca la aplicación.

## **4. Ejecutar la API**

En la terminal de Visual Studio Code ejecuta:

```powershell
dotnet restore
dotnet run
```

La API quedará disponible en:

```text
http://localhost:5270
```

Abre Swagger en el navegador:

```text
http://localhost:5270/swagger
```

Para detener la API, vuelve a la terminal y presiona `Ctrl + C`.

## **5. Endpoints**

### **GET /health**

Comprueba que la API está funcionando.

```powershell
curl.exe -i http://localhost:5270/health
```

Respuesta esperada: `200 OK`.

```json
{
  "status": "OK"
}
```

### **GET /api/products**

Devuelve todos los productos guardados en memoria.

```powershell
curl.exe -i http://localhost:5270/api/products
```

Respuesta esperada: `200 OK`.

### **GET /api/products/{id}**

Busca un producto por su identificador.

```powershell
curl.exe -i http://localhost:5270/api/products/1
```

Si el producto existe, devuelve `200 OK`. Si no existe, devuelve `404 Not Found`.

Prueba el caso que no existe:

```powershell
curl.exe -i http://localhost:5270/api/products/999
```

### **POST /api/orders**

Simula la compra de un producto. `Task.Delay` representa una espera para enseñar `async/await`; no realiza una operación real de entrada o salida.

En PowerShell ejecuta:

```powershell
curl.exe -i -X POST http://localhost:5270/api/orders `
  -H "Content-Type: application/json" `
  -d '{"productId":1,"quantity":2}'
```

Si los datos son correctos, devuelve `201 Created`. Una cantidad inválida o mayor al stock devuelve `400 Bad Request`. Un producto que no existe devuelve `404 Not Found`.

## **6. Flujo de una petición**

**1.** El cliente envía una petición HTTP, por ejemplo `GET /api/products/1`.

**2.** ASP.NET Core revisa la ruta y encuentra el controlador correspondiente.

**3.** El controlador recibe el parámetro `id` y llama a `ProductService`.

**4.** El servicio busca el objeto dentro de la lista en memoria.

**5.** El controlador crea una respuesta con un código HTTP y datos JSON.

**6.** El servidor devuelve la respuesta al cliente.

## **7. Códigos HTTP usados**

- `200 OK`: la consulta se completó correctamente.
- `201 Created`: la compra simulada se creó correctamente.
- `400 Bad Request`: los datos enviados no son válidos o no hay stock suficiente.
- `404 Not Found`: el producto solicitado no existe.

## **8. Ideas para la demostración**

- En `Product.cs`, identifica `int`, `string` y `decimal`.
- En `ProductService.cs`, crea objetos con `new Product` y revisa los métodos con parámetros.
- En `ProductsController.cs`, explica `[ApiController]`, `[Route]`, `[HttpGet]` y el parámetro `id`.
- En `OrdersController.cs`, revisa `[HttpPost]`, el cuerpo JSON y los códigos HTTP.
- En `OrderService.cs`, explica que `await` permite esperar sin bloquear el flujo de la aplicación.
- En Swagger, ejecuta cada endpoint y observa la petición y la respuesta.
