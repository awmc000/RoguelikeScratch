using Random = System.Random;

/*
 * GameDice
 *
 * Used by the GameManager to roll dice.
 * Eg. Roll(6, 1) rolls one d6 die for a
 * result between 1 and 6 inclusive.
 */
public class GameDice
{
    private Random _random;

    public GameDice()
    {
        _random = new Random();
    }
    public int Roll(int faces, int quantity)
    {
        int roll = 0;
        
        for (int i = 0; i < quantity; i++)
        {
            roll += _random.Next(1, faces + 1);
        }

        return roll;
    }

}
