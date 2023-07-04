using System;
using System.Linq;
using UnityEngine;
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

    public LevelGenerator levelGenerator;

    public GameDice Dice;

    public int Level;

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
        
        mobsOnScreen = FindObjectsOfType<Mob>();

        LogEvent("The Burrow Tale begins.");

        Level = 0;
        
        CreateLevel();
    }

    private void LogEvent(string eventString)
    {
        eventLog.LogEvent("[" + _turnsPassed + "]: " + eventString);
    }

    private int HowManyMobs()
    {
        return (int) Math.Round(0.9 * Level + 2);
    }
    public void CreateLevel()
    {
        Level++;
        levelGenerator.GenerateLevel();

        // TODO: Get rid of old mobs
        for (int i = 0; i < levelGenerator.InstantiatedMobsList.Count(); i++)
            Destroy(levelGenerator.InstantiatedMobsList[i]);
        
        // Place gate to the next level
        _oldTunnel = tunnel;
        int[] tunnelPosArr = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 tunnelPos = new Vector3(tunnelPosArr[0] + 0.5f, tunnelPosArr[1] + 0.5f, 0);
        tunnel = Instantiate(tunnel, tunnelPos, Quaternion.identity);
        Destroy(_oldTunnel);
        
        PlacePlayer();

        for (int i = 0; i < HowManyMobs(); i++)
            levelGenerator.InstantiatedMobsList.Add(CreateMob());
    }
    private void PlacePlayer()
    {
        int[] playerPos = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 playerPosVec = new Vector3(playerPos[0] + 0.5f, playerPos[1] + 0.5f, 0);
        player.transform.position = playerPosVec;
    }
    private GameObject CreateMob()
    {
        int[] mobSpotArr = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 mobSpotVec = new Vector3(mobSpotArr[0] + 0.5f, mobSpotArr[1] + 0.5f, 0);

        int whichMob = Dice.Roll(levelGenerator.MobList.Count(), 1) - 1;
        GameObject mob = levelGenerator.MobList[whichMob];
        return Instantiate(mob, mobSpotVec, Quaternion.identity);
    }

    public bool TileFree(Vector2 targetPos)
    {

        // Check if there is a blocked tile there.
        bool wallTile = tilemapCollider2D.OverlapPoint(targetPos);

        // Check if there is a mob there.
        bool mobThere = MobAtTile(targetPos);

        return ((!wallTile) && (!mobThere));
    }

    public bool TunnelAtTile(Vector2 targetPos)
    {
        return (Vector2)tunnel.transform.position == targetPos;
    }
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

    public bool CanFight(Mob mob)
    {
        Vector3 mobPos = mob.transform.position;
        Vector3 playerPos = player.transform.position;
        // Player and mob can fight if their positions differ by no more than 1 in x and y.
        bool xClose = Mathf.Abs(mobPos.x - playerPos.x) <= 1;
        bool yClose = Mathf.Abs(mobPos.y - playerPos.y) <= 1;

        return (xClose && yClose);
    }

    public void HurtPlayer(int dice)
    {
        LogEvent("A mob attacks you!");
        int roll = Dice.Roll(6, dice);
        LogEvent(dice + "D6 ROLL: " + roll);
        LogEvent("You lost " + roll + " hp!");
        player.ChangeHealth(-roll);
    }

    public void HurtMob(Mob target, int dice)
    {
        LogEvent("You attack the " + target.mobName + "!");
        int roll = Dice.Roll(6, dice);
        LogEvent(dice + "D6 ROLL: " + roll);
        LogEvent("Hit " + target.mobName + " for " + roll + " hp!");
        target.ChangeHealth(-roll);
        if (target.currentHealth <= 0)
        {
            Instantiate(target.drop, target.transform.position, Quaternion.identity);
            target.transform.gameObject.SetActive(false);
            int newMoney = Dice.Roll(6, target.lootMultiplier);
            player.money += newMoney;
            eventLog.LogEvent("Got " + newMoney + " coins.");
            UpdateMobsList();
        }
    }

    public Vector2 GetPlayerPos()
    {
        return player.transform.position;
    }

    public void UpdateMobsList()
    {
        mobsOnScreen = FindObjectsOfType<Mob>();
    }
    
    public void UpdateItemPickupsList()
    {
        itemPickupsOnScreen = FindObjectsOfType<ItemPickup>();
        Console.WriteLine("Found " + itemPickupsOnScreen.Count());
    }

    // Thanks Imprity from Unity Forum
    public static void GUIDrawSprite(Rect rect, Sprite sprite){
        UnityEngine.Rect spriteRect = sprite.rect;
        UnityEngine.Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, 
            new UnityEngine.Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, 
                spriteRect.width/ tex.width, spriteRect.height / tex.height));
    }

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

    /*
     * The Player calls this method when it is done its turn.
     * So this function handles the NPCs and any other phenomena and then advances the turn counter.
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

    public void GameOver()
    {
        eventLog.LogEvent("You died!");
        eventLog.CloseWriter();
    }

}
