using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class CreateGameRoomRequestDto
{
    [Required(ErrorMessage = "El nombre de la sala es obligatorio.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Range(2, 8, ErrorMessage = "La sala debe permitir entre 2 y 8 jugadores.")]
    public int MaxPlayers { get; set; }
}
