using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class CreatePlayerRequestDto
{
    [Required(ErrorMessage = "El nombre del jugador es obligatorio.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El nombre del jugador solo puede contener letras y números.")]
    public string Nombre { get; set; } = string.Empty;
}