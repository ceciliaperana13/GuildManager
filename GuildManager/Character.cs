public class Character
{
    public string name {get ; private set;}
    public int lvl {get ; private set;}
    public int health {get ; private set;}
    public int def {get ; private set;}
    public int magicAttack {get ; private set;}
    public int physicAttack {get ; private set;}
    public double power {get ; set;}
    public string image {get ; private set;}

    public Character(string name, int lvl, int health, int def, int magicAttack, int physicAttack, string image)
    {
        this.name = name;
        this.lvl = lvl;
        this.health = health;
        this.def = def;
        this.magicAttack = magicAttack;
        this.physicAttack = physicAttack;
        // this.power = this.refreshPower();
        this.image = image;
        this.refreshPower();
    }

    public Character(string name, string image) // pour les perso non combattants
    {
        this.name = name;
        this.image = image;
    }

    public virtual void refreshPower() {}
}