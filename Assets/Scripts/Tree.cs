using System;
using System.Collections.Generic;
using Random = System.Random;

/*
 *  Tree
 *
 *  Purpose: Generate dungeon maps for the level generator to use as a blueprint.
 *  Tree generates a 2 dimensional (jagged) array of numbers, like 0 for wall and 1 for floor.
 *  It does not handle anything to do with the Unity game engine. LevelGenerator takes
 *  maps generated by this class's methods and uses them to draw the tile map in the game.
 */
public class Tree
{
    // ====================================================
    // Data Members
    // ====================================================
    // Constants
    public const int DungeonWidth   = 80; // In tiles
    public const int DungeonHeight  = 40;
    
    private const int SplitMargin    = 1; // How many tiles between rooms
    private const int SplitIterations = 3; // Dungeon will have 2^SplitIterations. So if this is 3, 8 rooms.

    private const int CorridorWidth = 1; 
    
    public const int TileFlagFloor = 1;
    public const int TileFlagWall = 0;
    
    // Variables
    
    private TreeNode _root;
    private List<TreeNode> _leafNodes;
    private List<Area> _roomList;
    private int[,] _mapArray;

    private Random _random;
    
    // ====================================================
    // Constructor
    // ====================================================
    public Tree()
    {
        _root = new TreeNode(null);
        _root.Data = new Area(0, 0, DungeonWidth, DungeonHeight);
        _leafNodes = new List<TreeNode>();
        _leafNodes.Add(_root);
        _roomList = new List<Area>();
        _random = new Random();
    }
    
    // ====================================================
    // Binary Partition Algorithm Internal Methods
    // ====================================================
    
    /**
     * Splits a single node into two. Partitions its Area in a random
     * direction, calling `HalveVertically()` or `HalveHorizontally()`.
     */
    private void Split(TreeNode node)
    {
        TreeNode newLChild = new TreeNode(node);
        TreeNode newRChild = new TreeNode(node);
        
        node.SetLeftChild(newLChild);
        node.SetRightChild(newRChild);

        // choose a random direction : horizontal or vertical splitting
        bool splitVertically = _random.Next(10 + 1) < 5;
        if (splitVertically)
        {
            HalveVertically(node, newLChild, newRChild);
        }
        else
        {
            HalveHorizontally(node, newLChild, newRChild);
        }   
    }
    
    /**
     * Splits `node` into exact halves along a vertical line. No random generation of room dimensions.
     */
    private void HalveVertically(TreeNode node, TreeNode newLChild, TreeNode newRChild)
    {
        Area toDivide = node.Data;
        
        // Compute the left half of the area.
        Area leftHalf = new Area(toDivide.X, toDivide.Y, toDivide.W / 2, toDivide.H);
        
        // Compute the right half of the area.
        Area rightHalf = new Area(toDivide.X + (toDivide.W / 2), toDivide.Y, toDivide.W / 2, toDivide.H);
        
        // Assign the appropriate halved area to each new child node.
        newLChild.Data = leftHalf;
        newRChild.Data = rightHalf;
    }

    /**
     * Splits `node` into exact halves along a horizontal line. No random generation of room dimensions.
     */
    private void HalveHorizontally(TreeNode node, TreeNode newLChild, TreeNode newRChild)
    {
        Area toDivide = node.Data;
        
        // Compute the left half of the area.
        Area topHalf = new Area(toDivide.X, toDivide.Y, toDivide.W, toDivide.H / 2);
        
        // Compute the right half of the area.
        Area bottomHalf = new Area(toDivide.X, toDivide.Y + (toDivide.H / 2), toDivide.W, toDivide.H / 2);
        
        // Assign the appropriate halved area to each new child node.
        newLChild.Data = topHalf;
        newRChild.Data = bottomHalf;
    }
    
    /**
     * Splits each leaf node of the tree. The new number of lead nodes
     * will be equal to 2^depth. If there is only a root node, there
     * will be two leaf nodes, if there are 4 leaf nodes, there will
     * be eight.
     */
    private void SplitAll()
    {
        List<TreeNode> newLeafNodes = new List<TreeNode>();

        foreach (TreeNode leaf in _leafNodes)
        {
            Split(leaf);
        }

        foreach (TreeNode leaf in _leafNodes)
        {
            if (leaf.GetLeftChild() != null)
                newLeafNodes.Add(leaf.GetLeftChild());
            if (leaf.GetRightChild() != null)
                newLeafNodes.Add(leaf.GetRightChild());
        }

        // Replace the list containing the 2nd lowest level
        // with the one containing the new leaf level.
        _leafNodes = newLeafNodes;
    }

    /**
     * Takes a fully partitioned list of nodes with areas and put a room in each one.
     * Returns a list of Areas describing rooms. Essentially randomly carves out a room within
     * the bounds of each Area given by the Data members of nodes in the node list.
     */
    private void CreateRooms()
    {
        TreeNode currentNode;
        Area currentArea;
        // For each node in the node list:
        for (int i = 0; i < _leafNodes.Count; i++)
        {
            // Get current Area.
            currentNode = _leafNodes[i];
            currentArea = currentNode.Data;
            
            // Pick a random x and y offset, both within width/2 and height/2.
            int xOffset = _random.Next(1, currentArea.W / 2);
            int yOffset = _random.Next(1, currentArea.H / 2);
            
            // Determine right and upper bounds based on the selected room offsets
            int widthBound = currentArea.W - xOffset;
            int heightBound = currentArea.H - yOffset;
            
            // Select width and height within the bounds
            int roomWidth = _random.Next(widthBound / 2, widthBound - SplitMargin);
            int roomHeight = _random.Next(heightBound / 2, heightBound - SplitMargin);
            Area newRoom = new Area(
                currentArea.X + xOffset,
                currentArea.Y + yOffset,
                roomWidth,
                roomHeight);
            _roomList.Add(newRoom);
            
        }
    }
    
    /**
     * Takes a list of rooms and adds to the list a series of corridors between rooms.
     */
    private void CreateCorridors()
    {
        // Shuffle the list
        int n = _roomList.Count;
        while (n > 1)
        {
            n--;
            int k = _random.Next(n + 1);
            Area val = _roomList[k];
            _roomList[k] = _roomList[n];
            _roomList[n] = val;
        }
       
        // Corridors are stored in a separate list so that _roomList is not modified while it is being
        // read.
        List<Area> corridors = new List<Area>();

        for (int i = 0; i < _roomList.Count - 1; i++)
        {
            Area roomA = _roomList[i];
            Area roomB = _roomList[i + 1];

            Console.WriteLine("Point A:");
            Area pointA = RandPointWithin(roomA);
            
            Console.WriteLine("Point B");
            Area pointB = RandPointWithin(roomB);
            
            // Create a rectangular area that spans the width.
            Area widthSpan;
            int corridorLength;
            
            // If A is to the left of B
            if (pointA.X < pointB.X)
            {
                // then length is B.X - A.X
                corridorLength = pointB.X - pointA.X;
                widthSpan = new Area(pointA.X, pointA.Y, corridorLength, CorridorWidth);
            }
            // If B is to the left of A
            else
            {
                // then length is A.X - B.X
                corridorLength = pointA.X - pointB.X;
                widthSpan = new Area(pointB.X, pointB.Y, corridorLength, CorridorWidth);
            }
            
            corridors.Add(widthSpan);
            
            // Create a rectangular area that spans the height.
            Area heightSpan;
            int corridorHeight;
            // If A is above B
            if (pointA.Y < pointB.Y)
            {
                // then height is B.Y - A.Y
                corridorHeight = pointB.Y - pointA.Y;
                heightSpan = new Area(widthSpan.X + widthSpan.W, pointA.Y, CorridorWidth, corridorHeight);
            }
            // If B is above A
            else
            {
                // then height is A.Y - B.Y
                corridorHeight = pointA.Y - pointB.Y;
                heightSpan = new Area(widthSpan.X + widthSpan.W, pointB.Y, CorridorWidth, corridorHeight);
            }
            corridors.Add(heightSpan);
        }
        
        // Now that we are finished adding corridors, we can safely add them all to _roomList.
        foreach (Area corridor in corridors)
            _roomList.Add(corridor);
    }

    /**
     * Returns a random point within `room`.
     *
     * \param room The room to pick a point within.
     * \return `Area`, a random point within the room.
     */
    private Area RandPointWithin(Area room)
    {
        // Pick a random point, where:
        // 0 == Top
        // 1 == Bottom
        // 2 == Left
        // 3 == Right
        int side = _random.Next(4);

        Area point = new Area(0, 0, 0, 0);

        switch (side)
        {
            case 0:
                // pick a point on the top edge
                point.Y = room.Y + room.H;
                point.X = _random.Next(room.X, room.X + room.W);
                break;
            case 1:
                // pick a point on the bottom edge
                point.Y = room.Y;
                point.X = _random.Next(room.X, room.X + room.W);
                break;
            case 2:
                // pick a point on the left edge
                point.X = room.X;
                point.Y = _random.Next(room.Y, room.Y + room.H);
                break;
            case 3:
                // pick a point on the right edge
                point.X = room.X + room.W;
                point.Y = _random.Next(room.Y, room.Y + room.H);
                break;
        }
        Console.WriteLine("X: " + room.X + ", Y: " + room.Y);
        return point;
    }

    public List<Area> GetCandlePoints()
    {
        List<Area> CandlePoints = new List<Area>();
        // For each room
        foreach (Area room in _roomList)
        {
            if (room.W > 1 && room.H > 1)
                CandlePoints.Add(RandPointWithin(room));
        }
        return CandlePoints;
    }

    

    /**
     * The main feature of `Tree`. Generates an int array representing
     * a dungeon generated by the binary partition algorithm, with all
     * rooms connected by dog-leg hallways.
     *
     * \return A 2-dimensional jagged int array where value of
     * TileFlagFloor == 1 represents a floor tile and  TileFlagWall
     * == 0 represents a wall tile.
     */
    public int[,] GenerateMap()
    {
        // Splitting 3 times gets the best results
        for (int i = 0; i < SplitIterations; i++)
            SplitAll();
        
        // Generate rooms within the bounds of the partitioned areas
        CreateRooms();
        
        // Generate corridors to attempt to sequentially connect the rooms
        CreateCorridors();
        
        // Convert the rooms and corridors from a list of rooms' XYWH into a map with 0 for wall and 1 for floor
        _mapArray = MakeMapArr(_roomList);
        
        return _mapArray;
    }

    /*
     * Returns a spot with a floor tile, where a player, mob, or item could be placed.
     * Currently returns a 2-element array. A vector would be preferable, but this 
     * needs to work outside the editor.
     *
     * \return `int[]` of length 2, where [0] is x coordinate, and [1] is y coordinate.
     */
    public int[] GetEntitySpot()
    {
        Area room = _roomList[_random.Next(_roomList.Count)];
        int x = _random.Next(room.X, room.X + room.W);
        int y = _random.Next(room.Y, room.Y + room.H);
        int[] arr = new int[2];
        arr[0] = x;
        arr[1] = y;
        return arr;
    }
    
    private int[,] MakeMapArr(List<Area> areaList)
    {
        int[,] mapArr = new int[DungeonWidth, DungeonHeight];

        Console.WriteLine("Size of arr: " + mapArr.Length);

        Area currentArea;
        
        for (int i = 0; i < areaList.Count; i++)
        {
            currentArea = areaList[i];

            for (int y = 0; y < currentArea.H; y++)
            {
                for (int x = 0; x < currentArea.W; x++)
                {
                    mapArr[currentArea.X + x, currentArea.Y + y] = TileFlagFloor;
                }
            }
        }

        return mapArr;
    }

}
