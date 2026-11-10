using Bomberman.Api.Models;

namespace Bomberman.Api.DTO;

public static class GameSessionMapper
{
    public static GameSessionResponseDto ToResponse(GameSession game)
    {
        lock (game)
        {
            return new GameSessionResponseDto
            {
                Id = game.Id,
                RoomId = game.RoomId,
                Status = game.Status.ToString().ToLowerInvariant(),
                WinnerPlayerId = game.WinnerPlayerId,
                Map = game.Map.Cells.Select(row => row.Select(cell => (int)cell).ToArray()).ToArray(),
                Players = game.Players.OrderBy(player => player.PlayerId).Select(player =>
                    new PlayerPositionResponseDto
                    {
                        PlayerId = player.PlayerId,
                        X = player.X,
                        Y = player.Y,
                        Lives = player.Lives,
                        IsAlive = player.IsAlive
                    }).ToList(),
                Bombs = game.Bombs.Select(bomb => new GameBombResponseDto
                {
                    Id = bomb.Id,
                    OwnerPlayerId = bomb.OwnerPlayerId,
                    X = bomb.X,
                    Y = bomb.Y,
                    Range = bomb.Range,
                    PlacedAtUtc = bomb.PlacedAtUtc,
                    ExplodesAtUtc = bomb.ExplodesAtUtc
                }).ToList(),
                Explosions = game.Explosions.Select(explosion => new GameExplosionResponseDto
                {
                    Id = explosion.Id,
                    EndsAtUtc = explosion.EndsAtUtc,
                    Cells = explosion.Cells.Select(cell => new ExplosionCellResponseDto
                    {
                        X = cell.X,
                        Y = cell.Y
                    }).ToList()
                }).ToList()
            };
        }
    }
}
