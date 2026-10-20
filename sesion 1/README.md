# **Sesión 01: primera API de CodeSprint 2026**

En esta sesión creamos una API con C#, .NET 10 y ASP.NET Core. El proyecto usa controladores y contiene un primer endpoint de bienvenida.

## **1. Crear el proyecto desde cero**

Abre PowerShell y ejecuta estos comandos:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman"
New-Item -ItemType Directory -Path "sesiones - backend\sesion 1" -Force
cd "sesiones - backend\sesion 1"
dotnet new webapi --use-controllers --framework net10.0 --name CodeSprint.Session01.Api --output .
```

El parámetro `--use-controllers` indica que queremos una Web API basada en controladores. `--framework net10.0` selecciona .NET 10 y `--output .` crea el proyecto en la carpeta actual.

## **2. Estructura principal**

```text
sesion 1/
├── Contracts/
│   └── HelloResponse.cs
├── Controllers/
│   └── HelloController.cs
├── Properties/
│   └── launchSettings.json
├── CodeSprint.Session01.Api.csproj
├── CodeSprint.Session01.Api.http
└── Program.cs
```

`HelloController` atiende la petición y `HelloResponse` define la forma de la respuesta JSON.

## **3. Código del controlador**

```csharp
using CodeSprint.Session01.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeSprint.Session01.Api.Controllers;

[ApiController]
[Route("api/hello")]
public sealed class HelloController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HelloResponse>(StatusCodes.Status200OK)]
    public ActionResult<HelloResponse> Get()
    {
        return Ok(new HelloResponse("¡Hola, CodeSprint!"));
    }
}
```

## **4. Anotaciones principales**

- `[ApiController]` activa comportamientos pensados para APIs, como la validación automática de datos y respuestas HTTP claras cuando una petición no es válida.

- `[Route("api/hello")]` define la ruta base del controlador. En este caso será `/api/hello`.

- `[HttpGet]` indica que el método responde a peticiones HTTP GET.

- `[ProducesResponseType<HelloResponse>(StatusCodes.Status200OK)]` documenta que una petición correcta devuelve el estado HTTP 200 y un objeto `HelloResponse`.

## **5. Contrato del endpoint**

- Método: `GET`.

- Ruta: `/api/hello`.

- Cuerpo de la petición: no usa cuerpo.

- Autenticación: no requiere autenticación en esta sesión.

- Respuesta correcta: HTTP `200 OK` con contenido `application/json`.

```json
{
  "message": "¡Hola, CodeSprint!"
}
```

- Errores propios del endpoint: no se definieron errores de negocio. Una ruta inexistente devuelve HTTP `404 Not Found` mediante ASP.NET Core.

## **6. Ejecutar la API**

Desde la carpeta `sesion 1`, ejecuta:

```powershell
dotnet restore
dotnet run --launch-profile http
```

La API quedará disponible en `http://localhost:5268`.

## **7. Probar el endpoint**

En otra terminal de PowerShell, ejecuta:

```powershell
curl.exe http://localhost:5268/api/hello
```

También puedes abrir esta dirección en el navegador:

```text
http://localhost:5268/api/hello
```

La respuesta esperada es:

```json
{"message":"¡Hola, CodeSprint!"}
```

También puedes enviar la petición desde el archivo `CodeSprint.Session01.Api.http` si tu editor admite archivos HTTP.
