namespace Bomberman.Api.Models;

public enum MapCell
{
    Free = 0,
    Wall = 1
}

public sealed class GameMap
{
    public const int Size = 9;

    public MapCell[][] Cells { get; init; } = CreateInitialCells();

    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }

    public bool IsFree(int x, int y)
    {
        return IsInside(x, y) && Cells[y][x] == MapCell.Free;
    }

    private static MapCell[][] CreateInitialCells()
    {
        return
        [
            [MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Free, MapCell.Wall],
            [MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall, MapCell.Wall]
        ];
    }
}
