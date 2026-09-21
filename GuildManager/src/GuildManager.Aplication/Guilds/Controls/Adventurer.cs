namespace GuildManager.Aplication.Guilds.Controls;
public class Adventurer : Character
{
    public int id {get; private set;}
    public string job {get; private set;}
    // Inventory inventory;
    public List<string> debuff {get; private set;} //à changer par une list d'objet debuff ?
    public bool isHurted {get; private set;}
    public int goldPrice {get; private set;}
    public int foodPrice {get; private set;}

    public Adventurer(int id, string name, string job, int lvl, int health, int def, int magicAttack, int physicAttack, string image, List<string> debuff, bool isHurted, int goldPrice, int foodPrice) : base(name, lvl, health, def, magicAttack, physicAttack, image)
    {
        this.id = id;
        this.job = job;
        this.debuff = debuff;
        this.isHurted = isHurted;
        this.goldPrice = goldPrice;
        this.foodPrice = foodPrice;
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

    public void Write()
    {
        Console.WriteLine($"ID : {this.id}\nNom : {this.name}\nClasse : {this.job}\nNiveau : {this.lvl}\nSanté : {this.health}\nMagie : {this.magicAttack}\nPhysique : {this.physicAttack}\ndéfense : {this.def}\nImage : {this.image}\nPrix : {this.goldPrice}\nConsommation : {this.foodPrice}");
    }
}