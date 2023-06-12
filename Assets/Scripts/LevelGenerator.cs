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
    
    // floor tiles
    /*public Tile floorTileTopLeft;
    public Tile floorTileTopCentre;
    public Tile floorTileTopRight;
    
    public Tile floorTileCentreLeft;*/
    public Tile floorTile;
    /*public Tile floorTileCentreRight;

    public Tile floorTileBottomLeft;
    public Tile floorTileBottomCentre;
    public Tile floorTileBottomRight;*/
    
    // wall tiles
    /*public Tile wallTileTopLeft;
    public Tile wallTileTopCentre;
    public Tile wallTileTopRight;
    
    public Tile wallTileCentreLeft;*/
    public Tile wallTile;
    /*public Tile wallTileCentreRight;

    public Tile wallTileBottomLeft;
    public Tile wallTileBottomCentre;
    public Tile wallTileBottomRight;*/
    public Tree BinaryTree = new Tree();
    // private members
    private int[,] _mapArray;
    
    public void GenerateLevel()
    {
        // Set up a binary tree and use it to generate the level.
        //Tree binaryTree = new Tree();
        
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
                        wallMap.SetTile(position, wallTile);
                        break;
                    case 1: // floor
                        floorMap.SetTile(position, floorTile);
                        break;

                }
            }
        }
        
        // set up position vector of the bottom left point
        //Vector3Int roomPos = new Vector3Int(x, y, 0);
        // floors first
        //floorMap.BoxFill(roomPos, floorTile, roomPos.x, roomPos.y,
        //    roomPos.x + width, roomPos.y + height);
    }
}
