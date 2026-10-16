namespace DungeonCrawler;

abstract class LevelElement(int x, int y, char symbol, ConsoleColor color)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public char Symbol { get; } = symbol;
    public ConsoleColor Color { get; } = color;

    public void Draw()
    {
        if (X < 0 || Y < 0 || X >= Console.BufferWidth || Y >= Console.BufferHeight)
            return;

        Console.SetCursorPosition(X, Y);
        Console.ForegroundColor = Color;
        Console.Write(Symbol);
    }
}

class Wall(int x, int y) : LevelElement(x, y, '#', ConsoleColor.Gray)
{
    public bool Explored { get; set; }
}

class Door(int x, int y) : LevelElement(x, y, '>', ConsoleColor.Blue)
{
    public bool Explored { get; set; }
}
