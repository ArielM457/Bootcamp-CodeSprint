using Bomberman.Api.DTO;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bomberman.Api.Controllers;

[ApiController]
[Route("api/bombs")]
public sealed class BombsController(BombService bombService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BombResponseDto>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<BombResponseDto>> GetAll()
    {
        var bombs = bombService.GetAll().Select(ToResponse).ToList();
        return Ok(bombs);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<BombResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BombResponseDto> GetById(int id)
    {
        var bomb = bombService.GetById(id);
        if (bomb is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(bomb));
    }

    [HttpPost]
    [ProducesResponseType<BombResponseDto>(StatusCodes.Status201Created)]
    public ActionResult<BombResponseDto> Create(CreateBombRequestDto request)
    {
        var bomb = bombService.Create(
            request.PlayerId,
            request.PosicionX,
            request.PosicionY,
            request.Alcance);

        return CreatedAtAction(
            nameof(GetById),
            new { id = bomb.Id },
            ToResponse(bomb));
    }

    private static BombResponseDto ToResponse(Bomb bomb)
    {
        return new BombResponseDto
        {
            Id = bomb.Id,
            PlayerId = bomb.PlayerId,
            PosicionX = bomb.PosicionX,
            PosicionY = bomb.PosicionY,
            Alcance = bomb.Alcance
        };
    }
}
