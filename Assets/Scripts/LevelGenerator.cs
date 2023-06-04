using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    // data members
    public Tilemap floorMap;
    public Tilemap wallMap;
    public Tile floorTile;
    public Tile wallTile;

    public void generateLevel()
    {
        // Placeholder level gen: make a random sized room
        int x = -10;
        int y = -10;
        int width = Random.Range(4, 20);
        int height = Random.Range(4, 20); 

        // set up position vector of the bottom left point
        Vector3Int roomPos = new Vector3Int(x, y, 0);
        // floors first
        floorMap.BoxFill(roomPos, floorTile, roomPos.x, roomPos.y,
            roomPos.x + width, roomPos.y + height);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
