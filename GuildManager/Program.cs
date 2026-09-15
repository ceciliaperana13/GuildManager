class Program
{
    static void Main(string[] args)
    {
        QuestManager questManager = new QuestManager();
        Quest quest = questManager.generateQuest("Hunt", 5);
        Console.WriteLine(quest.name + "\n" + quest.description);
        Console.WriteLine("Aventuriers sélectionnés");

        CharacterGenerator caracterGenerator = new CharacterGenerator();
        Adventurer adventurer = caracterGenerator.generateCharacter(5);
        questManager.addAdventurerToQuest(quest.name, adventurer);
        foreach(Adventurer caracter in quest.adventurers)
        {
            Console.WriteLine($"Nom : {caracter.name}\nClasse : {caracter.job}\nNiveau : {caracter.lvl}\nSanté : {caracter.health}\nMagie : {caracter.magicAttack}\nPhysique : {caracter.physicAttack}\ndéfense : {caracter.def}\nImage : {caracter.image}\n");
        }

        // Adventurer adventurer = caracterGenerator.generateCaracter(1);
        Console.WriteLine($"Nom : {adventurer.name}\nClasse : {adventurer.job}\nNiveau : {adventurer.lvl}\nSanté : {adventurer.health}\nMagie : {adventurer.magicAttack}\nPhysique : {adventurer.physicAttack}\ndéfense : {adventurer.def}\nImage : {adventurer.image}");
    }
}
