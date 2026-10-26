using Bomberman.Api.Models;

namespace Bomberman.Api.Services;

public sealed class BombService
{
    private readonly List<Bomb> _bombs =
    [
        new Bomb { Id = 1, PlayerId = 1, PosicionX = 2, PosicionY = 3, Alcance = 2 },
        new Bomb { Id = 2, PlayerId = 2, PosicionX = 7, PosicionY = 5, Alcance = 3 }
    ];

    public List<Bomb> GetAll()
    {
        return _bombs;
    }

    public Bomb? GetById(int id)
    {
        return _bombs.FirstOrDefault(bomb => bomb.Id == id);
    }

    public Bomb Create(int playerId, int posicionX, int posicionY, int alcance)
    {
        var bomb = new Bomb
        {
            Id = _bombs.Count == 0 ? 1 : _bombs.Max(existingBomb => existingBomb.Id) + 1,
            PlayerId = playerId,
            PosicionX = posicionX,
            PosicionY = posicionY,
            Alcance = alcance
        };

        _bombs.Add(bomb);
        return bomb;
    }
}
