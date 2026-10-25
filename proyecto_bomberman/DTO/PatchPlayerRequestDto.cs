using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class PatchPlayerRequestDto
{
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El nombre del jugador solo puede contener letras y números.")]
    public string? Nombre { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La vida del jugador no puede ser negativa.")]
    public int? vida { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El puntaje del jugador no puede ser negativo.")]
    public double? puntaje { get; set; }
}