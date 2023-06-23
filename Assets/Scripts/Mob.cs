using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public string mobName;
    public int maxHealth;
    public int currentHealth;
    public int sightRadius;
    public int attackDamage;

    // If `tiresOut` is true, the mob will follow the player for
    // followDistance steps before giving up.
    public bool tiresOut;
    public int followDistance;
    int _followCounter;
    
    // Item drops
    public ItemPickup drop;
    
    // Number of d6 rolled for gold drop.
    public int lootMultiplier;
    public GameManager gameManager;

    Vector2 _targetPos;
    private Queue _path;

    public AudioSource attackSound;

    public int GetHealth()
    {
        return currentHealth;
    }

    public void ChangeHealth(int change)
    {
        currentHealth += change;
    }

    public void AttackPlayer()
    {
        gameManager.HurtPlayer(attackDamage);
    }

    // Uses a pathfinding algorithm to compute the shortest path from this to `destination`.
    // The path is returned as a queue, where each item is a step in the path, and the first
    // item out is the first step the mob needs to take.
    private Queue<Vector2> ShortestPath(Vector2 destination)
    {
        // TODO: Implement
        return null;
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
        if (gameManager.Dice.Roll(20, 1) > 14)
        {
            Vector3 target = new Vector3(gameManager.Dice.Roll(1, 1),
                gameManager.Dice.Roll(1, 1), 0);
            if (gameManager.TileFree(transform.position + target))
                transform.position += target;
        }
    }

    // Determines and executes the mob's action this turn.
    public void MakeMove()
    {
        // First priority: attack the player if touching.
        if (gameManager.CanFight(this))
        {
            AttackPlayer();
            return;
        }
        // Second priority: Walk toward the player if they are in sight radius.
        if (inSight() && (!tiresOut || _followCounter < followDistance))
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
