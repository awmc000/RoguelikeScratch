using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 *  LevelGenerator
 *
 *  Purpose: Draw the level onto the TileMap with the Tiles specified.
 *  Has a Tree type object, which does all the procedural generation.
 *  This class just iterates over data given by the Tree, and draws it
 *  to the game world.
 */
public class LevelGenerator : MonoBehaviour
{
    // Data members
    public Tilemap floorMap;
    public Tilemap wallMap;

    public Tile floorTile;
    public Tile wallTile;

    public Tree BinaryTree = new Tree();

    public GameObject Mob;
    public List<GameObject> MobList;
    public  List<GameObject> InstantiatedMobsList = new List<GameObject>();

    // private members
    private int[,] _mapArray;

    // Returns true if the given tile, touches a floor tile. 
    private bool TouchesFloor(int wallX, int wallY)
    {
        bool TL = false, T = false, TR = false, 
            L = false, R = false, 
            BL = false, B = false, BR = false;

        bool notTopBoundary = wallY > 1; 
        bool notBottomBoundary = wallY < Tree.DungeonHeight - 1;
        bool notLeftBoundary = wallX > 1;
        bool notRightBoundary = wallX < Tree.DungeonWidth - 1;

        if (notTopBoundary && notLeftBoundary)
            TL = _mapArray[wallX - 1, wallY - 1] == Tree.TileFlagFloor;
        
        if (notTopBoundary)
            T  = _mapArray[wallX,     wallY - 1] == Tree.TileFlagFloor;
        
        if (notTopBoundary && notRightBoundary)
            TR = _mapArray[wallX + 1, wallY - 1] == Tree.TileFlagFloor;

        if (notLeftBoundary)
            L  = _mapArray[wallX - 1, wallY    ] == Tree.TileFlagFloor;
        
        if (notRightBoundary)
            R  = _mapArray[wallX + 1, wallY    ] == Tree.TileFlagFloor;
        
        if (notBottomBoundary && notLeftBoundary)
            BL = _mapArray[wallX - 1, wallY + 1] == Tree.TileFlagFloor;
        
        if (notBottomBoundary)
            B  = _mapArray[wallX,     wallY + 1] == Tree.TileFlagFloor;
        
        if (notBottomBoundary && notRightBoundary)
            BR = _mapArray[wallX + 1, wallY + 1] == Tree.TileFlagFloor;
        
        return (TL || T || TR || 
                L  ||      R  ||
                BL || B || BR );
    }
    
    public void GenerateLevel()
    {
        // Set up a binary tree and use it to generate the level.
        BinaryTree = new Tree();

        // Clear tiles in case we aren't on the first level
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        
        // Get an int array from the binary tree.
        _mapArray = BinaryTree.GenerateMap();
        
        // Iterate over the map array
        Vector3Int position = new Vector3Int(0, 0, 0);
        for (int y = 0; y < Tree.DungeonHeight; y++)
        {
            for (int x = 0; x < Tree.DungeonWidth; x++)
            {
                position.x = x;
                position.y = y;
                switch (_mapArray[x, y])
                {
                    case 0: // wall
                        if (TouchesFloor(x,y))
                            wallMap.SetTile(position, wallTile);
                        break;
                    case 1: // floor
                        floorMap.SetTile(position, floorTile);
                        break;

                }
            }
        }
    }
}
