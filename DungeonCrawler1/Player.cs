namespace DungeonCrawler;

class Player(int x, int y)
: LevelElement(x, y, '@', ConsoleColor.Yellow), ICombatant
{
    public string Name => "Player";
    public int HP { get; set; } = 100;
    public Dice AttackDice { get; } = new Dice(2, 6, 2);
    public Dice DefenceDice { get; } = new Dice(2, 6, 0);
}
