namespace Bomberman.Api.DTO;

public sealed class PlayerResponseDto
{
    //public int Id {get; set;}
    public string Nombre {get; set;} = string.Empty;
    public int vida {get; set;}
    public double puntaje {get; set;}
}