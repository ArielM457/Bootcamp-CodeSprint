using Bomberman.Api.DTO;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bomberman.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public sealed class UsuariosController(UsuarioService usuarioService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var usuarios = (await usuarioService.GetAllAsync(cancellationToken))
            .Select(ToResponse)
            .ToList();

        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioService.GetByIdAsync(id, cancellationToken);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(usuario));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponseDto>> Create(
        CreateUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioService.CreateAsync(
            request.Nombre,
            request.Correo,
            request.Password,
            cancellationToken);

        if (usuario is null)
        {
            return Conflict(new { message = "Ya existe un usuario con ese correo." });
        }

        var response = ToResponse(usuario);

        return CreatedAtAction(
            nameof(GetById),
            new { id = usuario.Id },
            response);
    }

    private static UsuarioResponseDto ToResponse(Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            PlayerId = usuario.Player.Id
        };
    }
}
