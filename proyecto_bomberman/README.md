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

## **Comunicación en tiempo real con SignalR**

La sesión 08 agrega comunicación en tiempo real para las partidas. El Hub está disponible en esta ruta:

```text
http://localhost:5271/hubs/game
```

La conexión necesita un JWT válido. El Hub obtiene el jugador desde el claim `player_id`, por lo que el cliente no envía el identificador del jugador.

### **Métodos del Hub**

- `JoinGame(gameId)`: conecta al jugador con una partida y lo agrega al grupo de su sala.
- `MovePlayer(direction)`: solicita un movimiento. Las direcciones válidas son `up`, `down`, `left` y `right`.

El cliente debe ejecutar `JoinGame` antes de solicitar un movimiento. Cada conexión puede participar en una sola partida.

### **Eventos enviados por el servidor**

- `PlayerConnected(playerId)`: informa que una conexión válida entró en la partida.
- `PlayerDisconnected(playerId)`: informa que una conexión abandonó la partida.
- `PlayerMoved(playerId, x, y)`: informa la nueva posición confirmada por el servidor.
- `GameStateUpdated(gameState)`: envía el mapa y las posiciones actuales de todos los jugadores.

`gameState` tiene esta forma:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "roomId": 1,
  "status": "playing",
  "map": [[1, 1, 1], [1, 0, 1], [1, 1, 1]],
  "players": [
    {
      "playerId": 1,
      "x": 1,
      "y": 1
    }
  ]
}
```

El mapa real de la partida conserva sus dimensiones completas. El ejemplo muestra una versión reducida para explicar la estructura.

### **Ejemplo de conexión**

El siguiente ejemplo usa el cliente `@microsoft/signalr`. Solo muestra cómo consumir el Hub. La integración dentro de Angular corresponde a la sesión 09.

```javascript
import * as signalR from '@microsoft/signalr';

const token = 'JWT_DEL_JUGADOR';
const gameId = 'ID_DE_LA_PARTIDA';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5271/hubs/game', {
    accessTokenFactory: () => token,
  })
  .build();

connection.on('PlayerConnected', (playerId) => {
  console.log('Jugador conectado:', playerId);
});

connection.on('PlayerDisconnected', (playerId) => {
  console.log('Jugador desconectado:', playerId);
});

connection.on('PlayerMoved', (playerId, x, y) => {
  console.log('Movimiento confirmado:', playerId, x, y);
});

connection.on('GameStateUpdated', (gameState) => {
  console.log('Estado actualizado:', gameState);
});

await connection.start();
await connection.invoke('JoinGame', gameId);
await connection.invoke('MovePlayer', 'right');
```

El servidor llama a `GameService.Move` para validar cada movimiento. Si el movimiento no es válido, la llamada `MovePlayer` devuelve un error y no se envían `PlayerMoved` ni `GameStateUpdated`.

Los eventos se envían al grupo `room-{roomId}`. Los jugadores conectados a otras salas no reciben esas actualizaciones.
