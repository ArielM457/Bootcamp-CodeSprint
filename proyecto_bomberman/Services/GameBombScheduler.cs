using Bomberman.Api.DTO;
using Bomberman.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Bomberman.Api.Services;

public sealed class GameBombScheduler(
    IServiceScopeFactory scopeFactory,
    IHubContext<GameHub> hubContext,
    ILogger<GameBombScheduler> logger)
{
    public void Schedule(Guid gameId, int roomId, Guid bombId, DateTimeOffset explodesAtUtc)
    {
        _ = RunAsync(gameId, roomId, bombId, explodesAtUtc);
    }

    private async Task RunAsync(
        Guid gameId,
        int roomId,
        Guid bombId,
        DateTimeOffset explodesAtUtc)
    {
        try
        {
            var delay = explodesAtUtc - DateTimeOffset.UtcNow;
            if (delay > TimeSpan.Zero) await Task.Delay(delay);

            using var scope = scopeFactory.CreateScope();
            var gameService = scope.ServiceProvider.GetRequiredService<GameService>();
            var result = gameService.ExplodeBomb(gameId, bombId);
            if (!result.Updated || result.Game is null || result.ExplosionId is null) return;

            await BroadcastAsync(roomId, result.Game);
            await Task.Delay(GameService.ExplosionDuration);

            var game = gameService.RemoveExplosion(gameId, result.ExplosionId.Value);
            if (game is not null) await BroadcastAsync(roomId, game);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "No se pudo procesar la explosión de la bomba {BombId}.", bombId);
        }
    }

    private Task BroadcastAsync(int roomId, Models.GameSession game)
    {
        return hubContext.Clients
            .Group($"room-{roomId}")
            .SendAsync("GameStateUpdated", GameSessionMapper.ToResponse(game));
    }
}
