namespace DungeonCrawler;

abstract class Enemy(int x, int y, char symbol, ConsoleColor color, string name, int hp, Dice attackDice, Dice defenceDice)
: LevelElement(x, y, symbol, color), ICombatant
{
    public string Name { get; } = name;
    public int HP { get; set; } = hp;
    public Dice AttackDice { get; } = attackDice;
    public Dice DefenceDice { get; } = defenceDice;

    public abstract void Update(Game game);
}

class Rat(int x, int y)
: Enemy(x, y, 'r', ConsoleColor.Red, "Rat", 10, new Dice(1, 6, 3), new Dice(1, 6, 1))
{
    public override void Update(Game game)
    {
        (int x, int y)[] directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];
        (int x, int y) direction = directions[Random.Shared.Next(directions.Length)];
        game.TryMoveEnemy(this, direction.x, direction.y);
    }
}

class Snake(int x, int y)
    : Enemy(x, y, 's', ConsoleColor.Green, "Snake", 25, new Dice(3, 4, 2), new Dice(1, 8, 5))
{
    public override void Update(Game game)
    {
        if (Game.Distance(this, game.Player) > 2)
            return;

        int dX = X - game.Player.X;
        int dY = Y - game.Player.Y;

        if (Math.Abs(dX) >= Math.Abs(dY) && dX != 0)
            game.TryMoveEnemy(this, Math.Sign(dX), 0);
        else if (dY != 0)
            game.TryMoveEnemy(this, 0, Math.Sign(dY));
    }
}

class Boss(int x, int y)
: Enemy(x, y, 'B', ConsoleColor.DarkRed, "Boss", 100, new Dice(4, 6, 2), new Dice(1, 6, 2))
{
    private int turnCounter = 0;

    public override void Update(Game game)
    {
        if (turnCounter < 5 && Game.Distance(this, game.Player) <= 4)
        {
            int dX = X - game.Player.X;
            int dY = Y - game.Player.Y;

            if (Math.Abs(dX) >= Math.Abs(dY) && dX != 0)
                game.TryMoveEnemy(this, -Math.Sign(dX), 0);
            else if (dY != 0)
                game.TryMoveEnemy(this, 0, -Math.Sign(dY));
        }
        turnCounter++;
        if (turnCounter >= 7)
        {
            turnCounter = 0;
        }
    }
}
