using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    int _turnsPassed;

    Rect _labelRect;
    GUIStyle _labelStyle;
    public Font labelFont;
    public Player player;
    public TilemapCollider2D tilemapCollider2D;
    public EventLog eventLog;

    public Mob[] mobsOnScreen;
    public ItemPickup[] itemPickupsOnScreen;

    public LevelGenerator levelGenerator;

    private GameDice _dice;
    
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
        _dice = new GameDice();
        _turnsPassed = 0;

        _labelRect = new Rect(10, 10, 180, 16);
        _labelStyle = new GUIStyle();
        _labelStyle.font = labelFont;
        _labelStyle.fontSize = 16;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );

        mobsOnScreen = FindObjectsOfType<Mob>();

        eventLog.logEvent("The Burrow Tale begins.");

        levelGenerator.GenerateLevel();

        PlacePlayer();

        CreateMob();
        CreateMob();
        CreateMob();
    }

    private void PlacePlayer()
    {
        int[] playerPos = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 playerPosVec = new Vector3(playerPos[0] + 0.5f, playerPos[1] + 0.5f, 0);
        player.transform.position = playerPosVec;
    }
    private void CreateMob()
    {
        int[] mobSpotArr = levelGenerator.BinaryTree.GetEntitySpot();
        Vector3 mobSpotVec = new Vector3(mobSpotArr[0] + 0.5f, mobSpotArr[1] + 0.5f, 0);
        Instantiate(levelGenerator.Mob, mobSpotVec, Quaternion.identity);
    }

    // Uses a pathfinding algorithm to compute the shortest path from `from` to `to`.
    // The path is returned as a queue, where each item is a step in the path, and the first
    // item out is the first step the mob needs to take.
    public Vector2[,] ShortestPath(Vector2 start, Vector2 end)
    {
        Queue<Vector2> q = new Queue<Vector2>();
        
        q.Enqueue(start);

        bool[,] visited = new bool[80, 40];

        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 80; x++)
                visited[x, y] = false;
        }
        
        Vector2[,] prev = new Vector2[80, 40];

        while (q.Count > 0)
        {
            Vector2 node = q.Dequeue();
            Vector2[] neighbours = GetNeighbours(node);

            foreach (Vector2 next in neighbours)
            {
                if (!visited[(int) next.x, (int) next.y])
                {
                    q.Enqueue(next);
                    visited[(int) next.x, (int) next.y] = true;
                    prev[(int) next.x, (int) next.y] = node;
                }
            }
        }

        return prev;
    }

    private Vector2[] GetNeighbours(Vector2 tile)
    {
        return null;
    }

    private List<Vector2> ReconstructPath(Vector2 start, Vector2 end, Vector2[,] prev)
    {
        List<Vector2> path = new List<Vector2>();

        Vector2 at = end;

        while (path[path.Count] != start)
        {
            at = prev[(int)at.x, (int)at.y];
        }
        /*
        for (Vector2 at = end; at != null; at = prev[(int)at.x, (int)at.y])
        {
            path.Add(at);
        }*/

        path.Reverse();
        
        if (path[0] == start)
        {
            return path;
        }

        return null;
    }
    

    public bool TileFree(Vector2 targetPos)
    {

        // Check if there is a blocked tile there.
        bool wallTile = tilemapCollider2D.OverlapPoint(targetPos);

        // Check if there is a mob there.
        bool mobThere = MobAtTile(targetPos);

        return ((!wallTile) && (!mobThere));
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
        eventLog.logEvent("A mob attacks you!");
        int roll = _dice.Roll(6, dice);
        eventLog.logEvent(dice + "D6 ROLL: " + roll);
        eventLog.logEvent("You lost " + roll + " hp!");
        player.ChangeHealth(-roll);
    }

    public void HurtMob(Mob target, int dice)
    {
        eventLog.logEvent("You attack the " + target.mobName + "!");
        int roll = _dice.Roll(6, dice);
        eventLog.logEvent(dice + "D6 ROLL: " + roll);
        eventLog.logEvent("Hit " + target.mobName + " for " + roll + " hp!");
        target.ChangeHealth(-roll);
        if (target.currentHealth <= 0)
        {
            target.transform.gameObject.SetActive(false);
            //Destroy(target.transform.GetChild(0).gameObject);
            //Destroy(target);
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

    void OnGUI()
    {
        GUI.Box(_labelRect, "Turn " + _turnsPassed + "; HP: " + player.GetHealth()
            + "/" + player.maxHealth, _labelStyle);
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
        eventLog.logEvent("You died!");
        Destroy(player);
    }

}
