using Microsoft.AspNetCore.Mvc;
using Bomberman.Api.Models;
using Bomberman.Api.Services;

namespace Bomberman.Api.Controllers;

[ApiController]
[Route("api/players")]
public sealed class PlayersController : ControllerBase
{
private readonly PlayerService _playerService;

    public PlayersController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public ActionResult<List<Player>> GetAll()
    {
        return Ok(_playerService.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Player> GetById(int id)
    {
        var player = _playerService.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return Ok(player);
    }

    [HttpPost]
    public ActionResult<Player> Create(Player player)
    {
        var createdPlayer = _playerService.Create(player.Nombre.Trim(), player.vida, player.puntaje);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPlayer.Id },
            createdPlayer);
    }
}
