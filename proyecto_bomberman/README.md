# **Proyecto de Player para Bomberman**

Esta API usa .NET 10, ASP.NET Core, Entity Framework Core y PostgreSQL. Por ahora trabaja únicamente con la entidad `Player`.

Los jugadores ya no se guardan en memoria. `PlayerService` usa `BombermanDbContext` para leer y guardar los datos en PostgreSQL.

## **Estructura principal**

```text
proyecto_bomberman/
├── Controllers/
│   └── PlayersController.cs
├── Data/
│   ├── Configurations/
│   │   └── PlayerConfiguration.cs
│   └── BombermanDbContext.cs
├── DTO/
├── Models/
│   └── Player.cs
├── Services/
│   └── PlayerService.cs
├── appsettings.json
├── Bomberman.Api.csproj
└── Program.cs
```

## **Paquetes configurados**

El archivo `Bomberman.Api.csproj` ya declara estos paquetes:

- `Npgsql.EntityFrameworkCore.PostgreSQL`: conecta Entity Framework Core con PostgreSQL.
- `Microsoft.EntityFrameworkCore.Design`: permite crear y aplicar migraciones.

Después de instalar .NET 10 y PostgreSQL, restaura los paquetes:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\sesiones - backend\proyecto_bomberman"
dotnet restore
```

Instala la herramienta de migraciones una sola vez:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

Si ya la tienes instalada, puedes actualizarla:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.13
```

## **Configurar PostgreSQL**

Crea una base de datos llamada `bomberman`. La conexión se encuentra directamente en `appsettings.json`:

```json
"ConnectionStrings": {
  "PostgreSQL": "Host=localhost;Port=5432;Database=bomberman;Username=postgres;Password=postgres"
}
```

Si tu instalación usa otro usuario, contraseña, puerto o nombre de base de datos, cambia esos valores directamente en `appsettings.json`.

## **Crear la tabla**

Cuando los paquetes y PostgreSQL estén disponibles, crea la primera migración y aplícala:

```powershell
dotnet ef migrations add InitialPlayer
dotnet ef database update
```

La migración creará la tabla `players` con los campos `id`, `nombre`, `vida` y `puntaje`. De igual forma agregará los dos jugadores iniciales definidos en `PlayerConfiguration`.

## **Ejecutar la API**

```powershell
dotnet run
```

La configuración actual usa este perfil local:

```text
http://localhost:5271
```

## **Endpoints**

- `GET /api/players`: muestra todos los jugadores.
- `GET /api/players/{id}`: busca un jugador por su id.
- `POST /api/players`: crea un jugador.
- `PUT /api/players/{id}`: reemplaza los datos de un jugador.
- `PATCH /api/players/{id}`: actualiza los campos enviados.
- `DELETE /api/players/{id}`: elimina un jugador.

Ejemplo para crear un jugador:

```json
{
  "nombre": "Ariel"
}
```
