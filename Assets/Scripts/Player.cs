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

    public List<Item> Inventory;
    
    // Interface booleans
    public bool inventoryOpen;
    
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

    private void AttackMob()
    {
        Mob targetMob = gameManager.GetMobAtTile(_targetPosition);
        gameManager.HurtMob(targetMob, attackDamage);
    }

    // Start is called before the first frame update
    void Start()
    {
        _movementVector = new Vector3(0.0f, 0.0f, 0.0f);
        _targetPosition = new Vector2(0.0f, 0.0f);
        _currentHealth = maxHealth;
        Inventory = new List<Item>();
        inventoryOpen = false;
    }

    private void HandleInput()
    {
        if (!_debouncing)
        {
            // up, north
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad8));
                return;
            }

            // northwest
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad7));
                return;
            }

            // northeast
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad9));
                return;
            }
            
            // down, south
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad2));
                return;
            }

            // southwest
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad1));
                return;
            }

            // southeast
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad3));
                return;
            }
            
            // left, west
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad4));
                return;
            }

            // right, east
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad6));
                return;
            }

            // wait / pass turn
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad5));
                return;
            }
            
            // grab item
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.G));
            }
            
            // toggle inventory menu
            if (Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.I));
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

            // GRAB ITEM
            case KeyCode.G:
                PickupItem();
                break;
            
            // TOGGLE INVENTORY MENU
            case KeyCode.I:
                inventoryOpen = !inventoryOpen;
                break;
        }

        // The targetPosition vector stores where the player wants to go.
        // We check if the player can go there, and if so, make the move.

        if (gameManager.TileFree(_targetPosition))
        {
            transform.localPosition += _movementVector;
        }
        else if (gameManager.MobAtTile(_targetPosition))
        {
            AttackMob();
        }

        _movementVector.Set(0.0f, 0.0f, 0.0f);
        _targetPosition = transform.localPosition;

        gameManager.FinishTurn();

        yield return new WaitForSeconds(0.125f);

        _debouncing = false;
    }

    private void PickupItem()
    {
        ItemPickup itemPickup = gameManager.GetItemPickupAtTile(transform.position);
        Item item = itemPickup.GetItem();
        gameManager.eventLog.logEvent("Picked up " + item.Name + ".");
        itemPickup.transform.gameObject.SetActive(false);
        Inventory.Add(item);
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
