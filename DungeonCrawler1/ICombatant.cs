namespace DungeonCrawler;

interface ICombatant
{
    string Name { get; }
    int HP { get; set; }
    Dice AttackDice { get; }
    Dice DefenceDice { get; }
}
