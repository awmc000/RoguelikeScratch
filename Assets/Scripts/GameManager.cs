using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    int _turnsPassed;

    Rect _labelRect;
    Rect _inventoryRect;
    Rect[] _inventoryIconRects;
    GUIStyle _labelStyle;
    public Font labelFont;
    public Player player;
    public TilemapCollider2D tilemapCollider2D;
    public EventLog eventLog;

    public Mob[] mobsOnScreen;
    public ItemPickup[] itemPickupsOnScreen;
    public GameObject tunnel;
    private GameObject _oldTunnel;

    public List<GameObject> _oldGameObjects;

    public LevelGenerator levelGenerator;

    public GameDice Dice;

    [FormerlySerializedAs("Level")] public int level;

    private Texture2D MakeTex( int width, int height, Color col )
    {
        Color[] pix = new Color[width * height];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = col;
        }
        Texture2D result = new Texture2D( width, height );
        result.SetPixels( pix );
        result.Apply();
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        Dice = new GameDice();
        _turnsPassed = 0;

        _labelRect = new Rect(10, 10, 180, 16);
        _labelStyle = new GUIStyle();
        _labelStyle.font = labelFont;
        _labelStyle.fontSize = 16;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );

        _inventoryRect = new Rect(226, 10, 320, 80);
        
        // setup inventory icon rectangles
        _inventoryIconRects = new Rect[6];
        for (int i = 0; i < 6; i++)
        {
            _inventoryIconRects[i] = new Rect(200, 10 + (16 * i), 16, 16);
        }
        
        _oldGameObjects = new List<GameObject>();

        mobsOnScreen = FindObjectsOfType<Mob>();

        LogEvent("The Burrow Tale begins.");

        level = 0;
        
        CreateLevel();
    }

    /**
     * Adds a turn counter to an event string before calling the EventLog's
     * method to log the event.
     */
    public void LogEvent(string eventString)
    {
        eventLog.LogEvent("[" + _turnsPassed + "]: " + eventString);
    }

    /**
     * Calculates how many mobs a level of the dungeon should have. The
     * formula is to round up `2 + (0.9 * level)` to the nearest integer.
     * 
     * \return `int`, a recommended number of enemies to place in a level.
     */
    private int HowManyMobs()
    {
        return (int) Math.Round(0.9 * level + 2);
    }
    
    /**
     * Sets up a new level of the dungeon, destroying old mobs and creating new
     * ones, and placing the player somewhere random.
     */
    public void CreateLevel()
    {
        level++;
        levelGenerator.GenerateLevel(level);

        // Get rid of old mobs
        for (int i = 0; i < levelGenerator.InstantiatedMobsList.Count(); i++)
            Destroy(levelGenerator.InstantiatedMobsList[i]);
        
        // Get rid of old `GameObject`s
        foreach (GameObject obj in _oldGameObjects)
            Destroy(obj);
        
        // Place gate to the next level
        _oldTunnel = tunnel;
        int[] tunnelPosArr = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 tunnelPos = new Vector3(tunnelPosArr[0] + 0.5f, tunnelPosArr[1] + 0.5f, 0);
        tunnel = Instantiate(tunnel, tunnelPos, Quaternion.identity);
        
        Destroy(_oldTunnel);
        
        PlaceCandles();
        
        // Place the player in a random location
        PlacePlayer();

        for (int i = 0; i < HowManyMobs(); i++)
            levelGenerator.InstantiatedMobsList.Add(CreateMob());
    }

    /**
     * 
     */
    private void PlaceCandles()
    {
        // Put a candle in each room
        List<Area> candlePoints = levelGenerator.BinaryTree.GetCandlePoints();
        foreach (Area candlePoint in candlePoints)
        {
            _oldGameObjects.Add(GameObject.Instantiate(levelGenerator.Candle,
                new Vector3(candlePoint.X + 0.5f, candlePoint.Y + 0.5f, 0),
                Quaternion.identity));
        }
    }
    
    /**
     * Puts the player in a random location on the map obtained by `Tree.GetEntitySpot()`.
     */
    private void PlacePlayer()
    {
        int[] playerPos = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 playerPosVec = new Vector3(playerPos[0] + 0.5f, playerPos[1] + 0.5f, 0);
        player.transform.position = playerPosVec;
    }
    
    /**
     * Creates a new mob at a random location in the level. The mob is randomly selected
     * from a list that belongs to the `LevelGenerator` instance that this object owns.
     */
    private GameObject CreateMob()
    {
        int[] mobSpotArr = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 mobSpotVec = new Vector3(mobSpotArr[0] + 0.5f, mobSpotArr[1] + 0.5f, 0);

        List<GameObject> mobList = levelGenerator.SurfaceMobs;

        int tier = levelGenerator.GetLevelTier(level);

        switch (tier)
        {
            // Surface
            case 0:
                mobList = levelGenerator.SurfaceMobs;
                break;

            case 1:
                mobList = levelGenerator.TopsoilMobs;
                break;

            case 2:
                mobList = levelGenerator.DeepsoilMobs;
                break;

            case 3:
            case 4:
                mobList = levelGenerator.CavesMobs;
                break;
        }

        int whichMob = Dice.Roll(mobList.Count(), 1) - 1;
        GameObject mob = mobList[whichMob];
        mob.gameObject.GetComponent<Mob>().canMove = true;
        return Instantiate(mob, mobSpotVec, Quaternion.identity);
    }

    /**
     * Returns whether the given position on the tilemap is free.
     * \param targetPos An (x, y) position to check.
     * \return True if there is no mob or wall at the tile `targetPos`. 
     */
    public bool TileFree(Vector2 targetPos)
    {

        // Check if there is a blocked tile there.
        bool wallTile = tilemapCollider2D.OverlapPoint(targetPos);

        // Check if there is a mob there.
        bool mobThere = MobAtTile(targetPos);

        return ((!wallTile) && (!mobThere));
    }

    /**
     * Returns whether there is a tunnel to the next level at the given tile.
     * \param targetPos An (x, y) position to check.
     * \return True if there is a tile at the tile `targetPos`.
     */
    public bool TunnelAtTile(Vector2 targetPos)
    {
        return (Vector2)tunnel.transform.position == targetPos;
    }
    
    /**
     * Returns whether there is a mob at the given tile.
     * \param targetPos An (x, y) position to check.
     * \return True if there is a tile at the tile `targetPos`.
     */
    public bool MobAtTile(Vector2 targetPos)
    {
        UpdateMobsList();

        foreach (Mob mob in mobsOnScreen)
        {
            if (mob.transform.position == (Vector3)targetPos)
            {
                return true;
            }
        }
        return false;
    }

    /**
     * Used together with `MobAtTile()`. Given a `targetPos`, returns the mob
     * that is on that tile.
     * \param targetPos An (x, y) position to check.
     * \return Mob object.
     */
    public Mob GetMobAtTile(Vector2 targetPos)
    {
        foreach (Mob mob in mobsOnScreen)
        {
            if (mob.transform.position == (Vector3) targetPos)
            {
                return mob;
            }
        }
        return null;
    }

    /**
     * Returns the ItemPickup at `targetPos` if one is there.
     */
    public ItemPickup GetItemPickupAtTile(Vector2 targetPos)
    {
        UpdateItemPickupsList();

        foreach (ItemPickup ip in itemPickupsOnScreen)
        {
            if (ip.transform.position == (Vector3)targetPos)
            {
                return ip;
            }
        }

        return null;
    }

    /**
     * Returns true if the player and `mob` are in contact and therefore can fight.
     */
    public bool CanFight(Mob mob)
    {
        Vector3 mobPos = mob.transform.position;
        Vector3 playerPos = player.transform.position;
        // Player and mob can fight if their positions differ by no more than 1 in x and y.
        bool xClose = Mathf.Abs(mobPos.x - playerPos.x) <= 1;
        bool yClose = Mathf.Abs(mobPos.y - playerPos.y) <= 1;

        return (xClose && yClose);
    }

    /**
     * Inflicts damage on the player for a `Mob`.
     *
     * \param dice The amount of d6 dice to roll for damage.
     */
    public void HurtPlayer(int dice)
    {
        int roll = Dice.Roll(6, dice);
        LogEvent(dice + "D6 ROLL: " + roll);
        LogEvent("You lost " + roll + " hp!");
        player.ChangeHealth(-roll);
    }

    /**
     * Inflicts damage on a `Mob` for the player.
     *
     * \param target The mob to inflict damage upon
     * \param dice The amount of d6 dice to roll for damage.
     */
    public void HurtMob(Mob target, int dice)
    {
        LogEvent("You attack the " + target.mobName + "!");
        int roll = Dice.Roll(6, dice);
        LogEvent(dice + "D6 ROLL: " + roll);
        LogEvent("Hit " + target.mobName + " for " + roll + " hp!");
        target.ChangeHealth(-roll);
        if (target.currentHealth <= 0)
        {
            _oldGameObjects.Add(Instantiate(target.drop, target.transform.position, Quaternion.identity).gameObject);
            target.transform.gameObject.SetActive(false);
            int newMoney = Dice.Roll(6, target.lootMultiplier);
            player.money += newMoney;
            eventLog.LogEvent("Got " + newMoney + " coins.");
            UpdateMobsList();
        }
    }

    /**
     * Returns the player's position as a `Vector2`.
     */
    public Vector2 GetPlayerPos()
    {
        return player.transform.position;
    }

    /**
     * This method is used to make sure all `Mob` objects are in the
     * `mobsOnScreen` list.
     */
    public void UpdateMobsList()
    {
        mobsOnScreen = FindObjectsOfType<Mob>();
    }
    
    /**
     * This method is used to make sure all `ItemPickup` objects are in the
     * `itemPickupsOnScreen` list.
     */
    public void UpdateItemPickupsList()
    {
        itemPickupsOnScreen = FindObjectsOfType<ItemPickup>();
        Console.WriteLine("Found " + itemPickupsOnScreen.Count());
    }

    // Thanks Imprity from Unity Forum
    /**
     * Draws a sprite onto a `Rect` object for GUI uses.
     * 
     * Method written by Imprity on the Unity forums.
     */
    public static void GUIDrawSprite(Rect rect, Sprite sprite){
        UnityEngine.Rect spriteRect = sprite.rect;
        UnityEngine.Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, 
            new UnityEngine.Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, 
                spriteRect.width/ tex.width, spriteRect.height / tex.height));
    }

    /**
     * Draws the basic game HUD, namely the turn counter, player HP, and inventory if open.
     */
    void OnGUI()
    {
        GUI.Box(_labelRect, "Turn " + _turnsPassed + "; HP: " + player.GetHealth()
            + "/" + player.maxHealth, _labelStyle);

        if (player.inventoryOpen)
        {
            string invList = "";
            for (int i = 0; i < player.Inventory.Count(); i++)
            {
                Item it = player.Inventory[i];
                
                if (player.selectedItem == i)
                    invList += "SELECTED: ";

                invList += it.Name + ":" + it.Description + ",\n";
                GUIDrawSprite(_inventoryIconRects[i], it.Icon);
            }
            invList += "[" + player.money + " coins.]\n";
            GUI.Box(_inventoryRect, invList, _labelStyle);
        }
    }

    /**
     * Player calls this method when it is done its turn.
     * Handles the NPCs and any other phenomena and then advances the turn counter.
     */
    public void FinishTurn()
    {
        // Tell the NPCs to do their next move.
        foreach (Mob mob in mobsOnScreen)
        {
            mob.MakeMove();
        }
        // Advance the turn counter.
        _turnsPassed++;
    }

    /**
     * Prints the message indicating that the player has died.
     * Will be used for future death-related game logic.
     */
    public void GameOver()
    {
        LogEvent("You died!");
    }

}
