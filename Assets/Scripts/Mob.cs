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
        _targetPos = transform.position;
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
