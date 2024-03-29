using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    // ====================================================
    // Data Members
    // ====================================================
    public string mobName;
    public int maxHealth;
    public int currentHealth;
    public int sightRadius;
    public int attackDamage;

    // If `tiresOut` is true, the mob will follow the player for
    // followDistance steps before giving up.
    public bool canMove = false;
    public bool tiresOut;
    public int followDistance;
    int _followCounter;
    
    // Item drops
    public ItemPickup drop;
    public int dropRate = 50;
    
    public ItemPickup rareDrop;
    public int rareDropRate = 15;
    
    // Number of d6 rolled for gold drop.
    public int lootMultiplier;
    public GameManager gameManager;

    Vector2 _targetPos;
    private Queue _path;

    public AudioSource attackSound;

    // ====================================================
    // Accessor & Mutator Methods
    // ====================================================
    
    /**
     * Returns the health of this mob.
     * \return `int`, health points.
     */
    public int GetHealth()
    {
        return currentHealth;
    }

    /**
     * Adds a given integer to the mob's health points.
     * Add a negative value to subtract health.
     *
     * \param change amount to add to mob's health points
     */
    public void ChangeHealth(int change)
    {
        currentHealth += change;
    }
    
    // ====================================================
    // Other Methods
    // ====================================================

    public void AttackPlayer()
    {
        gameManager.LogEvent(mobName + " hit you!");
        gameManager.HurtPlayer(attackDamage);
    }

    public void moveToPlayer()
    {
        Vector2 playerPos = gameManager.GetPlayerPos();
        Vector2 myPos = transform.position;
        _targetPos = new Vector2(myPos.x, myPos.y);
        // Find whether to travel west, east, or neither
        float distX = playerPos.x - _targetPos.x;
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

        float distY = playerPos.y - _targetPos.y;

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
            FollowIncrement();
        }
        // Try making only the horizontal change
        else if (gameManager.TileFree(new Vector2(_targetPos.x, transform.position.y)))
        {
            transform.localPosition = new Vector2(_targetPos.x, transform.position.y);
            FollowIncrement();
        }
        // Try making only the vertical change
        else if (gameManager.TileFree(new Vector2(transform.position.x, _targetPos.y)))
        {
            transform.localPosition = new Vector2(transform.position.x, _targetPos.y);
            FollowIncrement();
        }

    }

    private void FollowIncrement()
    {
        if (tiresOut)
            _followCounter++;
    }

    /*
     * Returns a boolean indicating whether the player is within `sightRadius`.
     * \return is the player in sight?
     */
    public bool InSight()
    {
        Vector2 playerPos = gameManager.GetPlayerPos();
        Vector3 myPos = transform.position;
        float checkX = playerPos.x - myPos.x;
        float checkY = playerPos.y - myPos.y;

        return ((Mathf.Abs(checkX) <= sightRadius) &&
                (Mathf.Abs(checkY) <= sightRadius));
    }

    /*
     * Determines and returns the mob's drop for when they are killed.
     */
    public ItemPickup DropItem()
    {
        if (gameManager.Dice.Roll(100, 1) < rareDropRate)
        {
            return rareDrop;
        }

        if (gameManager.Dice.Roll(100, 1) < dropRate)
        {
            return drop;
        }

        return null;
    }
    
    /*
     * Mob behaviour logic: Move around randomly.
     */
    public void Bumble()
    {
        if (gameManager.Dice.Roll(20, 1) > 14)
        {
            Vector3 target = new Vector3(gameManager.Dice.Roll(1, 1),
                gameManager.Dice.Roll(1, 1), 0);
            if (gameManager.TileFree(transform.position + target))
                transform.position += target;
        }
    }

    /*
     * Determines and executes the mob's action this turn.
     */
    public void MakeMove()
    {
        if (!canMove)
            return;
        // First priority: attack the player if touching.
        if (gameManager.CanFight(this))
        {
            AttackPlayer();
            print(mobName + " attacks player.");
            return;
        }
        // Second priority: Walk toward the player if they are in sight radius.
        if (InSight() && (!tiresOut || _followCounter < followDistance))
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
        if (tiresOut)
            _followCounter = 0;
    }

}
