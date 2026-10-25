using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class ReplacePlayerRequestDto
{
    [Required(ErrorMessage = "El nombre del jugador es obligatorio.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El nombre del jugador solo puede contener letras y números.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La vida del jugador es obligatoria.")]
    [Range(0, int.MaxValue, ErrorMessage = "La vida del jugador no puede ser negativa.")]
    public int? vida { get; set; }

    [Required(ErrorMessage = "El puntaje del jugador es obligatorio.")]
    [Range(0, double.MaxValue, ErrorMessage = "El puntaje del jugador no puede ser negativo.")]
    public double? puntaje { get; set; }
}