namespace GuildManager.Aplication.Guilds.Controls;
public class Item
{
    public int id {get ; set;}
    public string name {get ; private set;}
    public int lvl {get ; private set;}
    public string type {get ; private set;}
    public int health {get ; private set;}
    public int def {get ; private set;}
    public int magicAttack {get ; private set;}
    public int physicAttack {get ; private set;}
    public string image {get ; private set;}
    public string description {get ; private set;}
    public int goldPrice {get ; private set;}

    public Item(int id, string name, string type, int health, int def, int magicAttack, int physicAttack, string image, string description, int goldPrice, int lvl)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.health = health;
        this.def = def;
        this.magicAttack = magicAttack;
        this.physicAttack = physicAttack;
        this.image = image;
        this.description = description;
        this.goldPrice = goldPrice;
        this.lvl = lvl;
    }
}