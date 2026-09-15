public class Adventurer : Character
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
