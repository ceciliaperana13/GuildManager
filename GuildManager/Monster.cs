public class Monster : Character // à développer ou supprimmer ?
{
    public Monster(string name, int lvl, int health, int def, int magicAttack, int physicAttack, string image) : base(name, lvl, health, def, magicAttack, physicAttack, image) {}
    

    public override void refreshPower()
    {
        this.power = this.health + this.magicAttack + this.physicAttack + this.def;
    }
}