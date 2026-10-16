namespace DungeonCrawler;

class Dice(int numberOfDice, int sidesPerDice, int modifier)
{
    public int NumberOfDice { get; } = numberOfDice;
    public int SidesPerDice { get; } = sidesPerDice;
    public int Modifier { get; } = modifier;

    public int Throw()
    {
        int total = Modifier;
        for (int i = 0; i < NumberOfDice; i++)
        {
            total += Random.Shared.Next(1, SidesPerDice + 1);
        }

        return total;
    }

    public override string ToString()
    {
        return $"{NumberOfDice}d{SidesPerDice}{(Modifier >= 0 ? "+" : "")}{Modifier}";
    }
}