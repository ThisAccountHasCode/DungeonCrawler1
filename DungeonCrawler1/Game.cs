namespace DungeonCrawler;

class Game(LevelData level)
{
    private const int VisionRange = 5;
    private string message = "Use arrow keys or WASD to move. Esc to quit.";

    public LevelData Level { get; } = level;
    public Player Player { get; } = new Player(level.PlayerStartX, level.PlayerStartY);

    public void Run()
    {
        bool isRunning = true;

        while (isRunning && Player.HP > 0)
        {
            Player.HP = 100; // GOD MODE TOGGLE FOR TESTING
            Draw();
            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape)
            {
                isRunning = false;
                continue;
            }

            MovePlayer(key);
            RemoveDefeatedEnemies();

            foreach (Enemy enemy in Level.Elements.OfType<Enemy>().ToList())
            {
                if (Player.HP <= 0)
                {
                    break;
                }

                if (enemy.HP <= 0)
                {
                    continue;
                }

                enemy.Update(this);
                RemoveDefeatedEnemies();
            }
        }
        Draw();
        Console.WriteLine(Player.HP <= 0 ? "\nGame over." : "\nGoodbye.");
        Console.CursorVisible = true;
    }

    private void Draw()
    {
        Console.Clear();

        foreach (Wall wall in Level.Elements.OfType<Wall>())
            if (Distance(wall, Player) <= VisionRange)
                wall.Explored = true;

        foreach (Door door in Level.Elements.OfType<Door>())
            if (Distance(door, Player) <= VisionRange)
                door.Explored = true;

        if (HasRoomForPositionedDrawing())
            DrawWithCursor();
        else
            DrawWithoutCursor();
    }

    private void MovePlayer(ConsoleKey key)
    {
        (int x, int y) = key switch
        {
            ConsoleKey.UpArrow or ConsoleKey.W => (0, -1),
            ConsoleKey.DownArrow or ConsoleKey.S => (0, 1),
            ConsoleKey.LeftArrow or ConsoleKey.A => (-1, 0),
            ConsoleKey.RightArrow or ConsoleKey.D => (1, 0),
            _ => (0, 0)
        };

        if (x != 0 || y != 0)
        {
            TryMovePlayer(x, y);
        }
    }

    private void TryMovePlayer(int dx, int dy)
    {
        int targetX = Player.X + dx;
        int targetY = Player.Y + dy;
        Enemy? enemy = Level.Elements.OfType<Enemy>()
            .FirstOrDefault(element => element.X == targetX && element.Y == targetY);

        if (enemy != null)
        {
            Fight(Player, enemy);
            return;
        }

        if (IsBlocked(targetX, targetY))
        {
            message = "You can't move there!";
            return;
        }

        Player.X = targetX;
        Player.Y = targetY;
        message = "";
    }

    public void TryMoveEnemy(Enemy enemy, int dx, int dy)
    {
        int targetX = enemy.X + dx;
        int targetY = enemy.Y + dy;

        if (targetX == Player.X && targetY == Player.Y)
        {
            Fight(enemy, Player);
            return;
        }

        if (!IsBlocked(targetX, targetY, enemy))
        {
            enemy.X = targetX;
            enemy.Y = targetY;
        }
    }

    public static double Distance(LevelElement first, LevelElement second)
    {
        int x = first.X - second.X;
        int y = first.Y - second.Y;
        return Math.Sqrt(x * x + y * y);
    }

    private bool IsBlocked(int x, int y, LevelElement? movingElement = null)
    {
        return x < 0 || x >= Level.Width || y < 0 || y >= Level.Height ||
                Level.Elements.Any(element => element != movingElement &&
                element.X == x && element.Y == y);
    }

    private void Fight(ICombatant attacker, ICombatant defender)
    {
        int attack = attacker.AttackDice.Throw();
        int defence = defender.DefenceDice.Throw();
        int damage = Math.Max(0, attack - defence);
        defender.HP -= damage;
        message = $"{attacker.Name} attacks {defender.Name}: {attack} - {defence} = {damage} damage.";

        if (defender.HP <= 0)
        {
            message += $" {defender.Name} is defeated.";
            return;
        }

        attack = defender.AttackDice.Throw();
        defence = attacker.DefenceDice.Throw();
        damage = Math.Max(0, attack - defence);
        attacker.HP -= damage;
        message += $" {defender.Name} counters: {attack} - {defence} = {damage} damage.";
    }

    private void RemoveDefeatedEnemies()
    {
        foreach (Enemy enemy in Level.Elements.OfType<Enemy>().Where(enemy => enemy.HP <= 0).ToList())
            Level.Remove(enemy);
    }


    private void DrawWithCursor()
    {
        foreach (Wall wall in Level.Elements.OfType<Wall>().Where(wall => wall.Explored))
            wall.Draw();

        foreach (Door door in Level.Elements.OfType<Door>().Where(door => door.Explored))
            door.Draw();

        foreach (Enemy enemy in Level.Elements.OfType<Enemy>())
        {
            if (Distance(enemy, Player) <= VisionRange)
                enemy.Draw();
        }

        Player.Draw();
        Console.ResetColor();
        int statusY = Math.Min(Level.Elements.Max(element => element.Y) + 1, Console.BufferHeight - 1);
        Console.SetCursorPosition(0, statusY);
        Console.Write($"HP: {Player.HP}   {message}");
    }

    private void DrawWithoutCursor()
    {
        int rightmost = Math.Max(Player.X, Level.Elements.Max(element => element.X));
        int lowest = Math.Max(Player.Y, Level.Elements.Max(element => element.Y));

        for (int y = 0; y <= lowest; y++)
        {
            for (int x = 0; x <= rightmost; x++)
            {
                LevelElement? element = GetVisibleElement(x, y);

                if (element == null)
                {
                    Console.Write(' ');
                    continue;
                }

                Console.ForegroundColor = element.Color;
                Console.Write(element.Symbol);
            }

            Console.WriteLine();
        }

        Console.ResetColor();
        Console.WriteLine($"HP: {Player.HP}   {message}");
    }

    private LevelElement? GetVisibleElement(int x, int y)
    {
        if (Player.X == x && Player.Y == y)
            return Player;

        Wall? wall = Level.Elements.OfType<Wall>()
            .FirstOrDefault(element => element.Explored && element.X == x && element.Y == y);
        if (wall != null)
            return wall;

        Door? door = Level.Elements.OfType<Door>()
            .FirstOrDefault(element => element.Explored && element.X == x && element.Y == y);
        if (door != null)
            return door;

        Enemy? enemy = Level.Elements.OfType<Enemy>()
            .FirstOrDefault(element => element.X == x && element.Y == y && Distance(element, Player) <= VisionRange);
        return enemy;
    }

    private bool HasRoomForPositionedDrawing()
    {
        int rightmost = Math.Max(Player.X, Level.Elements.Max(element => element.X));
        int lowest = Math.Max(Player.Y, Level.Elements.Max(element => element.Y));
        return rightmost < Console.BufferWidth && lowest + 1 < Console.BufferHeight;
    }
}
