# **Proyecto base de Player para Bomberman**

Este proyecto contiene un ejemplo inicial de una API de jugadores con .NET 10 y ASP.NET Core.

El modelo `Player` contiene solamente `Id` y `Nombre`. Los jugadores se guardan en una lista en memoria, por lo que los datos se reinician cuando se detiene la aplicación.

## Endpoints

- `GET /api/players`: muestra los jugadores.
- `GET /api/players/{id}`: busca un jugador por su id.
- `POST /api/players`: agrega un jugador. El id se asigna automáticamente.

Ejemplo de cuerpo para `POST /api/players`:

```json
{
  "nombre": "Ariel"
}
```

La lista empieza con dos jugadores de ejemplo.

## Estructura

```text
proyecto_bomberman/
├── Controllers/
│   └── PlayersController.cs
├── Models/
│   └── Player.cs
├── Services/
│   └── PlayerService.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── Bomberman.Api.csproj
├── Program.cs
└── README.md
```

## **Ejecutar la base**

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\sesiones - backend\proyecto_bomberman"
dotnet restore
dotnet run
```

La aplicación inicia en:

```text
http://localhost:5271
```
