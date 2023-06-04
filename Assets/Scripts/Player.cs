using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    // data members
    bool debouncing;
    Vector3 movementVector;
    Vector2 targetPosition;
    public TilemapCollider2D tilemapCollider2D;
    public GameManager gameManager;

    public int maxHealth;
    int currentHealth;
    public int attackDamage;

    // Setters and getters
    public int getHealth()
    {
        return currentHealth;
    }

    public void changeHealth(int change)
    {
        currentHealth += change;
    }

    public void setHealth(int newHealth)
    {
        currentHealth = newHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        movementVector = new Vector3(0.0f, 0.0f, 0.0f);
        targetPosition = new Vector2(0.0f, 0.0f);
        currentHealth = maxHealth;
    }

    private void handleInput()
    {
        if (!debouncing)
        {
            // up, north
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad8));
            }

            // northwest
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad7));
            }

            // northeast
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad9));
            }
            
            // down, south
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad2));
            }

            // southwest
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad1));
            }

            // southeast
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad3));
            }
            
            // left, west
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad4));
            }

            // right, east
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad6));
            }

            // wait / pass turn
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad5));
            }
        }

    }

    IEnumerator DebounceCoroutine(KeyCode key)
    {
        switch (key)
        {
            // N
            case KeyCode.Keypad8:
                movementVector.Set(0.0f, 1.0f, 0.0f);
                targetPosition += Vector2.up;
                break;

            // NW
            case KeyCode.Keypad7:
                movementVector.Set(-1.0f, 1.0f, 0.0f);
                targetPosition += Vector2.up + Vector2.left;
                break;

            // NE
            case KeyCode.Keypad9:
                movementVector.Set(1.0f, 1.0f, 0.0f);
                targetPosition += Vector2.up + Vector2.right;
                break;

            // S
            case KeyCode.Keypad2:
                movementVector.Set(0.0f, -1.0f, 0.0f);
                targetPosition += Vector2.down;
                break;

            // SW
            case KeyCode.Keypad1:
                movementVector.Set(-1.0f, -1.0f, 0.0f);
                targetPosition += Vector2.down + Vector2.left;
                break;

            // SE
            case KeyCode.Keypad3:
                movementVector.Set(1.0f, -1.0f, 0.0f);
                targetPosition += Vector2.down + Vector2.right;
                break;

            // W
            case KeyCode.Keypad4:
                movementVector.Set(-1.0f, 0.0f, 0.0f);
                targetPosition += Vector2.left;
                break;

            // E
            case KeyCode.Keypad6:
                movementVector.Set(1.0f, 0.0f, 0.0f);
                targetPosition += Vector2.right;
                break;

            // WAIT
            case KeyCode.Keypad5:
                break;

        }

        // The targetPosition vector stores where the player wants to go.
        // We check if the player can go there, and if so, make the move.

        if (gameManager.TileFree(transform, targetPosition))
        {
            transform.localPosition += movementVector;
        }
        else if (gameManager.MobAtTile(targetPosition))
        {
            Mob targetMob = gameManager.GetMobAtTile(targetPosition);
            targetMob.changeHealth(-attackDamage);
            gameManager.eventLog.logEvent("Player hit " + targetMob.mobName + " for " + attackDamage 
                + " damage, it now has " + targetMob.currentHealth);
            if (targetMob.currentHealth <= 0)
            {
                Destroy(targetMob.transform.GetChild(0).gameObject);
                Destroy(targetMob);
                gameManager.UpdateMobsList();
            }
        }

        movementVector.Set(0.0f, 0.0f, 0.0f);
        targetPosition.x = transform.localPosition.x;
        targetPosition.y = transform.localPosition.y;

        gameManager.FinishTurn();

        yield return new WaitForSeconds(0.125f);

        debouncing = false;
    }

    // Update is called once per frame
    void Update()
    {
        handleInput();

        // check if I'm dead!
        if (currentHealth <= 0)
        {
            gameManager.gameOver();
        }
    }
}
