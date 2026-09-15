public class Caracter
{
    public string name {get ; private set;}
    public int lvl {get ; private set;}
    public int health {get ; private set;}
    public int def {get ; private set;}
    public int magicAttack {get ; private set;}
    public int physicAttack {get ; private set;}
    public double power {get ; set;}
    public string image {get ; private set;}

    public Caracter(string name, int lvl, int health, int def, int magicAttack, int physicAttack, string image)
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

    public Caracter(string name, string image) // pour les perso non combattants
    {
        this.name = name;
        this.image = image;
    }

    public virtual void refreshPower() {}
}

public class Adventurer : Caracter
{
    public string job {get; private set;}
    //Inventory inventory;
    public List<string> debuff {get; private set;} //à changer par une list d'objet debuff ?
    public bool isHurted {get; private set;}


    public Adventurer(string name, string job, int lvl, int health, int def, int magicAttack, int physicAttack, string image, List<string> debuff, bool isHurted) : base(name, lvl, health, def, magicAttack, physicAttack, image)
    {
        this.job = job;
        this.debuff = debuff;
        this.isHurted = isHurted;  
        refreshPower();
    }

    public override void refreshPower()
    {
        if (this.job == "Mage")
            this.power = this.health*0.5 + this.magicAttack*2 + this.physicAttack*0.5 + this.def*0.5;
        else if (this.job == "Tank")
            this.power = this.health*1.5 + this.magicAttack*0.5 + this.physicAttack * 0.5 + this.def * 1.5;
        else
            this.power = this.health*0.5 + this.magicAttack*0.5 + this.physicAttack * 1.5 + this.def * 1.5;
    }
}

public class Monster : Caracter // à développer ou supprimmer ?
{
    public Monster(string name, int lvl, int health, int def, int magicAttack, int physicAttack, string image) : base(name, lvl, health, def, magicAttack, physicAttack, image) {}
    

    public override void refreshPower()
    {
        this.power = this.health + this.magicAttack + this.physicAttack + this.def;
    }
}