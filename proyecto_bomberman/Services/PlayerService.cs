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

    public Player Create(string nombre)
    {
        var player = new Player
        {
            Id = _players.Count == 0 ? 1 : _players.Max(existingPlayer => existingPlayer.Id) + 1,
            Nombre = nombre
        };

        _players.Add(player);
        return player;
    }

    public Player? Replace(int id, string nombre, int vida, double puntaje)
    {
        var player = GetById(id);
        if (player is null)
        {
            return null;
        }

        player.Nombre = nombre;
        player.vida = vida;
        player.puntaje = puntaje;
        return player;
    }

    public Player? Update(int id, string? nombre, int? vida, double? puntaje)
    {
        var player = GetById(id);
        if (player is null)
        {
            return null;
        }

        if (nombre is not null)
        {
            player.Nombre = nombre;
        }

        if (vida.HasValue)
        {
            player.vida = vida.Value;
        }

        if (puntaje.HasValue)
        {
            player.puntaje = puntaje.Value;
        }

        return player;
    }

    public bool Delete(int id)
    {
        var player = GetById(id);

        if (player is null)
        {
            return false;
        }

        _players.Remove(player);
        return true;
    }
}
