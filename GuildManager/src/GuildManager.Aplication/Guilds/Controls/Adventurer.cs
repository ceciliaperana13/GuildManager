using Microsoft.AspNetCore.StaticAssets;

namespace GuildManager.Aplication.Guilds.Controls;
public class Adventurer : Character
{
    public int id {get; private set;}
    public string job {get; private set;}
    // Inventory inventory;
    public List<string> debuff {get; private set;} //à changer par une list d'objet debuff ?
    public bool isHurted {get; set;}
    public bool isDead {get; set;}
    public int hurtTurn {get; set;}
    public int goldPrice {get; private set;}
    public int foodPrice {get; private set;}
    public List<Item> items {get; private set;}
    public int xp {get; set;}

    public Adventurer(int id, string name, string job, int lvl, int xp, int health, int def, int magicAttack, int physicAttack, string image, List<string> debuff, bool isHurted, int hurtTurn, bool isDead, int goldPrice, int foodPrice) : base(name, lvl, health, def, magicAttack, physicAttack, image)
    {
        this.id = id;
        this.job = job;
        this.debuff = debuff;
        this.isHurted = isHurted;
        this.hurtTurn = hurtTurn;
        this.isDead = isDead;
        this.goldPrice = goldPrice;
        this.foodPrice = foodPrice;
        this.xp = xp;
        this.hurtTurn = 0;
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

    public void levelUp()
    {
        while (this.xp >= this.lvl*10)
        {
            this.xp -= this.lvl*10;
            this.lvl++;
            Console.WriteLine($"{this.name} est passé niveau {this.lvl}");
            this.assignPoints();
        }
    }

    public void assignPoints()
    {
        int points = 20;
        Random random = new Random();

        for (int i = 0; i < points; i++)
        {
            int roll = random.Next(100);

            if (this.job == "mage")
            {
                if (roll < 50)       // 50%
                    this.magicAttack++;
                else if (roll < 80)  // 30%
                    this.health++;
                else if (roll < 90)  // 10%
                    this.def++;
                else                 // 10%
                    this.physicAttack++;
            }
            else if (this.job == "tank")
            {
                if (roll < 40)       // 40%
                    this.def++;
                else if (roll < 80)  // 40%
                    this.health++;
                else if (roll < 90)  // 10%
                    this.magicAttack++;
                else                 // 10%
                    this.physicAttack++;
            }
        else
        {
            if (roll < 30)           // 30%
                    this.health++;
                else if (roll < 70)  // 40%
                    this.physicAttack++;
                else if (roll < 90)  // 20%
                    this.def++;
                else                 // 10%
                    this.magicAttack++;
        }
    }
    refreshPower();
    }

    public void adventurerHurt(int turn)
    {
        this.isHurted = true;
        this.hurtTurn = turn;
    }

    public void AdventurerHeal()
    {
        this.isHurted = false;
        this.hurtTurn = 0;
        Console.WriteLine($"{this.name} s'est blessé au combat {this.isHurted} {this.hurtTurn}");
    }
}