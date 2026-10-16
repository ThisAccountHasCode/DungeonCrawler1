using DungeonCrawler;

class Program
{
    static void Main()
    {
        Console.Title = "Dungeon Crawler";
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        var level = new LevelData();
        level.Load(Path.Combine(AppContext.BaseDirectory, "Level1.txt"));

        var game = new Game(level);
        game.Run();
    }
}
