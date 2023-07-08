using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    // ====================================================
    // Data Members
    // ====================================================
    
    // Reference to gameManager to send messages back
    public GameManager gameManager;
    
    // Movement
    bool _debouncing;
    Vector3 _movementVector;
    Vector2 _targetPosition;
    

    // HP & death
    public int maxHealth;
    private int _currentHealth;
    private bool _dead = false;

    // Attack info
    public int attackDamage; //! Number of D6 dice to roll for player's attack damage.
    public AudioSource attackSound;

    public bool inventoryOpen;
    public List<Item> Inventory;
    public int selectedItem = 0;
    public int money = 0;
    
    /*!< Tracks whether the last key pressed was escape. If it was, another press
     * will take the player to the main menu and abandon the game. */
    public bool hitEscapeLast; 
    public enum InputMode
    {
        Movement,
        Inventory,
        Looking
    }
    public InputMode mode = InputMode.Movement;
    
    // Setters and getters
    
    /**
     * Accessor method that returns the player's health points as an `int`.
     * \return `int`, player's current health points
     */
    public int GetHealth()
    {
        return _currentHealth;
    }

    /**
     * Mutator method that adds the `int` parameter `change` to the player's health.
     *
     * Game logic for a health potion will pass a positive value and game logic for
     * damage would pass a negative value.
     * \param change Value to add to player health points.
     */
    public void ChangeHealth(int change)
    {
        _currentHealth += change; 
        _currentHealth %= maxHealth + 1;
    }

    /**
     * Mutator method that sets the player's health to the int parameter `newHealth`.
     * \param newHealth the value to set player health points to.
     */
    public void SetHealth(int newHealth)
    {
        _currentHealth = newHealth;
    }

    private void AttackMob()
    {
        Mob targetMob = gameManager.GetMobAtTile(_targetPosition);
        gameManager.HurtMob(targetMob, attackDamage);
    }

    /**
     * As with any MonoBehaviour, start is called before the first frame update.
     * Sets up the movement and target position vectors, sets the current HP to
     * the maximum, constructs the inventory, and sets `inventoryOpen` to `false`.
     */
    void Start()
    {
        _movementVector = new Vector3(0.0f, 0.0f, 0.0f);
        _targetPosition = new Vector2(0.0f, 0.0f);
        _currentHealth = maxHealth;
        Inventory = new List<Item>();
        inventoryOpen = false;
    }

    /**
     * Uses debouncing to accept keypresses and start coroutines (`DebounceCoroutine()`)
     * to handle them.
     */
    private void HandleInput()
    {
        if (!_debouncing)
        {
            // toggle inventory menu
            if (Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.I));
                hitEscapeLast = false;
            }

            // hit escape twice to quit
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Escape));
            }

            if (_dead)
                return;

            // north
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad8));
                hitEscapeLast = false;
                return;
            }

            // northwest
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad7));
                hitEscapeLast = false;
                return;
            }

            // northeast
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad9));
                hitEscapeLast = false;
                return;
            }
            
            // down, south
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad2));
                if (inventoryOpen && Inventory.Count > 1)
                    selectedItem = (selectedItem + 1) % Inventory.Count;
                hitEscapeLast = false;
                return;
            }

            // southwest
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad1));
                hitEscapeLast = false;
                return;
            }

            // southeast
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad3));
                hitEscapeLast = false;
                return;
            }
            
            // left, west
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad4));
                hitEscapeLast = false;
                return;
            }

            // right, east
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad6));
                hitEscapeLast = false;
                return;
            }

            // wait / pass turn
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.Keypad5));
                hitEscapeLast = false;
                return;
            }
            
            // grab item
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.G));
                hitEscapeLast = false;
            }
            
            
            // use selected item
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(DebounceCoroutine(KeyCode.KeypadEnter));
                hitEscapeLast = false;
            }
            
            // TODO: toggle "Look" mode
            /*
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(DebounceCoroutine(Keycode.K));
            }
             */
        }

    }

    /**
     * Given `key`, which identifies which key has been pressed,
     * executes the appropriate game logic. This is a coroutine.
     *
     * \param key The key which has been pressed, such as `KeyCode.Keypad8`
     */
    IEnumerator DebounceCoroutine(KeyCode key)
    {
        // If the player moves or attacks, advance the turn counter.
        // HUD manipulations can be done indefinitely in the same turn.
        bool advancedTurn = false;
        _targetPosition = transform.position;
        switch (key)
        {
            // N
            case KeyCode.Keypad8:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(0.0f, 1.0f, 0.0f);
                    _targetPosition += Vector2.up;
                    advancedTurn = true;
                }
                break;

            // NW
            case KeyCode.Keypad7:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(-1.0f, 1.0f, 0.0f);
                    _targetPosition += Vector2.up + Vector2.left;
                    advancedTurn = true;
                }
                break;

            // NE
            case KeyCode.Keypad9:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(1.0f, 1.0f, 0.0f);
                    _targetPosition += Vector2.up + Vector2.right;
                    advancedTurn = true;
                }
                break;

            // S
            case KeyCode.Keypad2:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(0.0f, -1.0f, 0.0f);
                    _targetPosition += Vector2.down;
                    advancedTurn = true;
                }
                break;

            // SW
            case KeyCode.Keypad1:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(-1.0f, -1.0f, 0.0f);
                    _targetPosition += Vector2.down + Vector2.left;
                    advancedTurn = true;
                }
                break;

            // SE
            case KeyCode.Keypad3:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(1.0f, -1.0f, 0.0f);
                    _targetPosition += Vector2.down + Vector2.right;
                    advancedTurn = true;
                }
                break;

            // W
            case KeyCode.Keypad4:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(-1.0f, 0.0f, 0.0f);
                    _targetPosition += Vector2.left;
                    advancedTurn = true;
                }
                break;

            // E
            case KeyCode.Keypad6:
                if (mode == InputMode.Movement)
                {
                    _movementVector.Set(1.0f, 0.0f, 0.0f);
                    _targetPosition += Vector2.right;
                    advancedTurn = true;
                }
                break;

            // WAIT
            case KeyCode.Keypad5:
                advancedTurn = true;
                if (gameManager.TunnelAtTile(transform.position))
                    gameManager.CreateLevel();
                break;

            // GRAB ITEM
            case KeyCode.G:
                if (gameManager.GetItemPickupAtTile(transform.position) != null)
                    PickupItem();
                break;
            
            // TOGGLE INVENTORY MENU
            case KeyCode.I:
                inventoryOpen = !inventoryOpen;
                if (mode == InputMode.Inventory)
                {
                    mode = InputMode.Movement;
                }
                else
                {
                    mode = InputMode.Inventory;
                }
                break;
            
            // USE INVENTORY ITEM
            case KeyCode.KeypadEnter:
                if (inventoryOpen)
                {
                    UseItem(Inventory[selectedItem]);
                    Inventory.Remove(Inventory[selectedItem]);
                    selectedItem = 0;
                }
                break;
            
            case KeyCode.Escape:
                if (hitEscapeLast)
                {
                    gameManager.eventLog.LogEvent("Exiting the game.");
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    gameManager.eventLog.LogEvent("Hit ESC again to exit the game.");
                    hitEscapeLast = true;
                }

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

        if (advancedTurn)
            gameManager.FinishTurn();

        yield return new WaitForSeconds(0.125f);

        _debouncing = false;
    }

    /**
     * Given an item which the player has just used, applies the appropriate changes
     * to player's location, stats, etc.
     *
     * \param item The item which the player has just used. Each `ItemPickup` object
     * owns an `Item` object that is used here.
     */
    private void UseItem(Item item)
    {
        if (item.Stats.ContainsKey("hp"))
        {
            _currentHealth = (_currentHealth + item.Stats["hp"]) % (maxHealth + 1);
            gameManager.eventLog.LogEvent("Restored " + item.Stats["hp"] + " hp.");
        }

        if (item.Stats.ContainsKey("dmg"))
        {
            attackDamage += item.Stats["dmg"];
            gameManager.eventLog.LogEvent("Increased damage by " + item.Stats["dmg"] + ".");
        }

        if (item.Flags.ContainsKey("teleport") && item.Flags["teleport"])
        {
            int[] gopt = gameManager.levelGenerator.BinaryTree.GetEntitySpot();
            transform.position = new Vector3((float)gopt[0] + 0.5f, (float)gopt[1] + 0.5f, 0f);
        }
    }
    
    /**
     * Assuming that there is an item where the player is standing on the tilemap,
     * adds it to the player's inventory.
     */
    private void PickupItem()
    {
        ItemPickup itemPickup = gameManager.GetItemPickupAtTile(transform.position);
        Item item = itemPickup.GetItem();
        gameManager.eventLog.LogEvent("Picked up " + item.Name + ".");
        itemPickup.transform.gameObject.SetActive(false);
        Inventory.Add(item);
    }

    /**
     * If the player is dead, ends the game, also flips the sprite and ceases animating
     * it to visually communicate that the player is dead.
     */
    private void CheckDead()
    {
        if (_currentHealth <= 0 && !_dead)
        {
            _dead = true;
            gameManager.GameOver();
            GetComponentInChildren<SpriteRenderer>().flipY = true;
            GetComponentInChildren<Animator>().enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();

        // check if I'm dead!
        CheckDead();
    }
}
