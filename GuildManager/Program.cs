class Program
{
    static void Main(string[] args)
    {
        // QuestManager questManager = new QuestManager();
        // Quest quest = questManager.generateQuest("Hunt", 5);
        // Console.WriteLine(quest.name + "\n" + quest.description);
        // Console.WriteLine("Aventuriers sélectionnés");

        // CharacterGenerator characterGenerator = new CharacterGenerator();
        // Adventurer adventurer = caracterGenerator.generateCharacter(5);
        // questManager.addAdventurerToQuest(quest.name, adventurer);
        // foreach(Adventurer caracter in quest.adventurers)
        // {
        //     Console.WriteLine($"ID : {caracter.id}\nNom : {caracter.name}\nClasse : {caracter.job}\nNiveau : {caracter.lvl}\nSanté : {caracter.health}\nMagie : {caracter.magicAttack}\nPhysique : {caracter.physicAttack}\ndéfense : {caracter.def}\nImage : {caracter.image}\n");
        // }

        CharacterGenerator characterGenerator = new CharacterGenerator();
        Adventurer adventurer = characterGenerator.generateCharacter(10);
        // Console.WriteLine($"ID : {adventurer.id}\nNom : {adventurer.name}\nClasse : {adventurer.job}\nNiveau : {adventurer.lvl}\nSanté : {adventurer.health}\nMagie : {adventurer.magicAttack}\nPhysique : {adventurer.physicAttack}\ndéfense : {adventurer.def}\nImage : {adventurer.image}");
        characterGenerator.addNewAdventurer(adventurer);

    }
}
