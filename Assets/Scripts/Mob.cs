using UnityEngine;

public class Mob : MonoBehaviour
{
    public string mobName;
    public int maxHealth;
    public int currentHealth;
    public int sightRadius;
    public GameManager gameManager;

    Vector2 _targetPos;

    public AudioSource attackSound;

    public int GetHealth()
    {
        return currentHealth;
    }

    public void ChangeHealth(int change)
    {
        currentHealth += change;
    }

    public void SetHealth(int newHealth)
    {
        currentHealth = newHealth;
    }

    public void AttackPlayer()
    {
        gameManager.HurtPlayer(1);
        gameManager.eventLog.logEvent(mobName + " hit you for 1 dmg.");
    }

    public void moveToPlayer()
    {
        Vector2 playerPos = gameManager.GetPlayerPos();
        _targetPos = transform.position;
        // Find whether to travel west, east, or neither
        float distX = playerPos.x - transform.position.x;
        // if negative, mob -> player (mob needs to go east)
        // if positive, player <- mob (mob needs to go west)
        // if zero, neither
        if (distX < 0)
        {
            _targetPos.x -= 1;
        }
        else if (distX > 0)
        {
            _targetPos.x += 1;
        }

        float distY = playerPos.y - transform.position.y;

        if (distY < 0)
        {
            _targetPos.y -= 1;
        }
        else if (distY > 0)
        {
            _targetPos.y += 1;
        }

        if (gameManager.TileFree(_targetPos))
        {
            transform.localPosition = _targetPos;
        }

    }

    // is the player in sight?
    public bool inSight()
    {
        Vector2 playerPos = gameManager.GetPlayerPos();
        Vector3 myPos = transform.position;
        float checkX = playerPos.x - myPos.x;
        float checkY = playerPos.y - myPos.y;

        return ((Mathf.Abs(checkX) <= sightRadius) &&
                (Mathf.Abs(checkY) <= sightRadius));
    }

    // Move around randomly.
    public void Bumble()
    {

    }

    // Determines and executes the mob's action this turn.
    public void MakeMove()
    {
        // First priority: attack the player if touching.
        if (gameManager.CanFight(this))
        {
            AttackPlayer();
            attackSound.Play();
            return;
        }
        // Second priority: Walk toward the player if they are in sight radius.
        if (inSight())
        {
            moveToPlayer();
            return;
        }
        
        // Third priority: Bumble around if neither touching or in sight radius.
        Bumble();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _targetPos = new Vector2(0.0f, 0.0f);
        currentHealth = maxHealth;
    }

}
