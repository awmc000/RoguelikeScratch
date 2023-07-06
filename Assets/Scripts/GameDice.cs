using Random = System.Random;

/**
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
    
    /**
     * Simulates rolling the die/dice: generates a random number
     * between 1 and `faces * quantity`.
     *
     * \param faces how many faces the dice have
     * \param quantity how many dice to roll
     * \return `int`, random number that is in the range [1, quantity * faces]
     */
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
