using System.ComponentModel.DataAnnotations;

namespace Bomberman.Api.DTO;

public sealed class JoinGameRoomByCodeRequestDto
{
    [Required]
    [StringLength(8, MinimumLength = 8)]
    public string Code { get; set; } = string.Empty;
}
