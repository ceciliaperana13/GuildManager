using System.ComponentModel;

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game("test", 0, 0, 1000, 1000, 1, 0);
        bool running = true;
        while (running)
        {
            //game.turnInProgress = true;
            game.turn ++;
            game.playTurn();
            if (game.Win())
                running = false;
            
        }
            
    }
}
