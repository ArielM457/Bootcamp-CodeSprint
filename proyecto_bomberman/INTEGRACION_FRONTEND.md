# **Integración del backend con el frontend**

## **Lo que tenemos**

El backend ya permite registrar e iniciar sesión, crear y consultar salas, unirse a una sala, iniciar una partida y mover jugadores. De igual forma tiene el Hub `/hubs/game` con `JoinGame` y `MovePlayer` para enviar movimientos en tiempo real.

Endpoints principales disponibles:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/rooms`
- `GET /api/rooms/{id}`
- `POST /api/rooms`
- `POST /api/rooms/{id}/join`
- `POST /api/games/{roomId}/start`
- `GET /api/games/{gameId}`
- `POST /api/games/{gameId}/move`

## **Endpoints que faltan**

**1.** `GET /api/players/me` OPCIONAL

Recibe el JWT y devuelve el perfil del jugador autenticado con `id`, `displayName`, `fullName`, `nickname` y `avatarId`.

**2.** `PATCH /api/players/me`OPCIONAL

Recibe `fullName`, `nickname` y `avatarId`. Debe actualizar y devolver el perfil sin recibir el `playerId`, porque sale del JWT.

**3.** `GET /api/rooms/current`

Recibe el JWT y devuelve la sala actual del jugador. Permite que el frontend muestre Volver a la sala aunque esté llena o jugando.

**4.** `PATCH /api/rooms/{roomId}`

Recibe `name`, `maxPlayers`, `arenaId` y `mode`. Solo el anfitrión puede modificar una sala que sigue en espera. Devuelve la sala actualizada.

**5.** `POST /api/rooms/{roomId}/bots`

No necesita datos del jugador en el cuerpo. Usa el JWT, agrega un Bombot si el jugador es anfitrión y devuelve la sala actualizada.

**6.** `DELETE /api/rooms/{roomId}/participants/me` OPCIONAL

Usa el JWT para retirar al jugador. Si sale el anfitrión, el backend debe asignar otro jugador humano. Si no quedan jugadores humanos, debe eliminar la sala.

**7.** `POST /api/rooms/{roomId}/return-to-waiting`

Se llama cuando termina la partida. Debe conservar los participantes, cambiar la sala a `waiting` y devolver la sala actualizada.

**8.** `GET /api/games/{gameId}/scores`

Devuelve el resultado y los puntos calculados por el servidor. El frontend no debe decidir los puntos ni indicar quién ganó.

**9.** `GET /api/leaderboard`

Devuelve `rank`, jugador, nombre, puntos, victorias y partidas jugadas.

## **SignalR pendiente**

El Hub también necesita:

- `PlaceBomb(commandId)`: coloca una bomba usando la partida y la posición confirmadas por el servidor.
- `RequestSnapshot()`: devuelve el estado completo después de una reconexión.
- `RestartGame(commandId)`: reinicia la partida si esa regla se mantiene en el frontend.

Cada movimiento o bomba debe recibir un `commandId`. Si la acción falla, el servidor debe enviar `CommandRejected`. Si funciona, debe enviar un `GameStateUpdated` con versión, mapa, jugadores, vidas, bombas, explosiones, tiempo y ganador.

## **Datos que deben coincidir**

Una sala debe devolver `hostPlayerId`, `arenaId`, `mode` y el tipo de cada participante, ya sea `player` o `bot`.

Al iniciar una partida, el backend debe devolver el `gameId`. El frontend debe conservar ese valor para conectarse a `/hubs/game` y ejecutar `JoinGame(gameId)`.
