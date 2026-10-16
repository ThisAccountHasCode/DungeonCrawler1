namespace DungeonCrawler;

class LevelData
{
    private readonly List<LevelElement> elements = [];

    public IReadOnlyList<LevelElement> Elements => elements;
    public int PlayerStartX { get; private set; }
    public int PlayerStartY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Load(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        elements.Clear();
        Width = lines.Max(line => line.Length);
        Height = lines.Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                switch (lines[y][x])
                {
                    case '#': elements.Add(new Wall(x, y)); break;
                    case 'r': elements.Add(new Rat(x, y)); break;
                    case 's': elements.Add(new Snake(x, y)); break;
                    case 'B': elements.Add(new Boss(x, y)); break;
                    case '>': elements.Add(new Door(x, y)); break;
                    case '<': elements.Add(new Door(x, y)); break;
                    case '@':
                        PlayerStartX = x;
                        PlayerStartY = y;
                        break;
                }
            }
        }
    }

    public void Remove(LevelElement element)
    {
        elements.Remove(element);
    }
}
