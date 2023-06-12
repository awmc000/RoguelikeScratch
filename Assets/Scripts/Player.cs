using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    // data members
    bool _debouncing;
    Vector3 _movementVector;
    Vector2 _targetPosition;
    public TilemapCollider2D tilemapCollider2D;
    public GameManager gameManager;

    public int maxHealth;
    int _currentHealth;
    public int attackDamage;

    public AudioSource attackSound;

    // Setters and getters
    public int GetHealth()
    {
        return _currentHealth;
    }

    public void ChangeHealth(int change)
    {
        _currentHealth += change;
    }

    public void SetHealth(int newHealth)
    {
        _currentHealth = newHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        _movementVector = new Vector3(0.0f, 0.0f, 0.0f);
        _targetPosition = new Vector2(0.0f, 0.0f);
        _currentHealth = maxHealth;
    }

    private void HandleInput()
    {
        if (!_debouncing)
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
                _movementVector.Set(0.0f, 1.0f, 0.0f);
                _targetPosition += Vector2.up;
                break;

            // NW
            case KeyCode.Keypad7:
                _movementVector.Set(-1.0f, 1.0f, 0.0f);
                _targetPosition += Vector2.up + Vector2.left;
                break;

            // NE
            case KeyCode.Keypad9:
                _movementVector.Set(1.0f, 1.0f, 0.0f);
                _targetPosition += Vector2.up + Vector2.right;
                break;

            // S
            case KeyCode.Keypad2:
                _movementVector.Set(0.0f, -1.0f, 0.0f);
                _targetPosition += Vector2.down;
                break;

            // SW
            case KeyCode.Keypad1:
                _movementVector.Set(-1.0f, -1.0f, 0.0f);
                _targetPosition += Vector2.down + Vector2.left;
                break;

            // SE
            case KeyCode.Keypad3:
                _movementVector.Set(1.0f, -1.0f, 0.0f);
                _targetPosition += Vector2.down + Vector2.right;
                break;

            // W
            case KeyCode.Keypad4:
                _movementVector.Set(-1.0f, 0.0f, 0.0f);
                _targetPosition += Vector2.left;
                break;

            // E
            case KeyCode.Keypad6:
                _movementVector.Set(1.0f, 0.0f, 0.0f);
                _targetPosition += Vector2.right;
                break;

            // WAIT
            case KeyCode.Keypad5:
                break;

        }

        // The targetPosition vector stores where the player wants to go.
        // We check if the player can go there, and if so, make the move.

        if (gameManager.TileFree(transform, _targetPosition))
        {
            transform.localPosition += _movementVector;
        }
        else if (gameManager.MobAtTile(_targetPosition))
        {
            Mob targetMob = gameManager.GetMobAtTile(_targetPosition);
            targetMob.changeHealth(-attackDamage);
            attackSound.PlayDelayed(0.5f);
            gameManager.eventLog.logEvent("Player hit " + targetMob.mobName + " for " + attackDamage 
                + " damage, it now has " + targetMob.currentHealth);
            if (targetMob.currentHealth <= 0)
            {
                Destroy(targetMob.transform.GetChild(0).gameObject);
                Destroy(targetMob);
                gameManager.UpdateMobsList();
            }
        }

        _movementVector.Set(0.0f, 0.0f, 0.0f);
        _targetPosition = transform.localPosition;
        //_targetPosition.x = transform.localPosition.x;
        //_targetPosition.y = transform.localPosition.y;

        gameManager.FinishTurn();

        yield return new WaitForSeconds(0.125f);

        _debouncing = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        // check if I'm dead!
        if (_currentHealth <= 0)
        {
            gameManager.GameOver();
        }
    }
}
