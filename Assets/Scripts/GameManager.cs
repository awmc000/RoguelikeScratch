using System.Collections;
using System.Collections.Generic;
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

    public LevelGenerator levelGenerator;

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
        _turnsPassed = 0;

        _labelRect = new Rect(10, 10, 180, 16);
        _labelStyle = new GUIStyle();
        _labelStyle.font = labelFont;
        _labelStyle.fontSize = 16;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );

        mobsOnScreen = FindObjectsOfType<Mob>();

        eventLog.logEvent("Set up the GameManager.");

        //levelGenerator.generateLevel();
    }

    public bool TileFree(Transform myPos, Vector2 targetPos)
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
            if (new Vector2(mob.transform.position.x, mob.transform.position.y) == targetPos)
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
            if (new Vector2(mob.transform.position.x, mob.transform.position.y) == targetPos)
            {
                return mob;
            }
        }
        // should never be reached, this is a stupid temporary solution
        return new Mob();
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

    public void HurtPlayer(int damage)
    {
        player.changeHealth(-damage);
    }

    public void hurtMob(Mob target, int damage)
    {
        target.changeHealth(-damage);
    }

    public Vector2 GetPlayerPos()
    {
        return player.transform.position;
    }

    public void UpdateMobsList()
    {
        mobsOnScreen = FindObjectsOfType<Mob>();
    }

    void OnGUI()
    {
        GUI.Box(_labelRect, "Turn " + _turnsPassed + "; HP: [" + new string('█', player.getHealth())
            + "]", _labelStyle);
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
            mob.makeMove();
        }
        // Advance the turn counter.
        _turnsPassed++;
    }

    public void gameOver()
    {
        eventLog.logEvent("You died!");
        Destroy(player);
    }

}
