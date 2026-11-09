using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class MovePlayerRequestDto
{
    [Required(ErrorMessage = "La dirección es obligatoria.")]
    public string Direction { get; set; } = string.Empty;
}
