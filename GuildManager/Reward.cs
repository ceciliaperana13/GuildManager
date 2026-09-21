public class Reward
{
    public int gold { get; set; }
    public int food { get; set; }
    public int prestige { get; set; }
    public List<Item> items { get; set; } = new();

    public Reward(int gold, int food, int prestige, List<Item> items)
    {
        this.gold = gold;
        this.food = food;
        this.prestige = prestige;
        this.items = items;
    }
}