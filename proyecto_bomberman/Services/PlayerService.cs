using Bomberman.Api.Models;

namespace Bomberman.Api.Services;

public sealed class PlayerService
{
private readonly List<Player> _players =
    [
        new Player { Id = 1, Nombre = "Jugador 1", puntaje = 0.0, vida = 3 },
        new Player { Id = 2, Nombre = "Jugador 2", puntaje = 0.0, vida = 3 }
    ];

    public List<Player> GetAll()
    {
        return _players;
    }

    public Player? GetById(int id)
    {
        return _players.FirstOrDefault(player => player.Id == id);
    }

    public Player Create(string nombre, int vida, double puntaje )
    {
        var player = new Player
        {
            Id = _players.Count == 0 ? 1 : _players.Max(existingPlayer => existingPlayer.Id) + 1,
            Nombre = nombre,
            vida = vida,
            puntaje = puntaje
        };

        _players.Add(player);
        return player;
    }
}
