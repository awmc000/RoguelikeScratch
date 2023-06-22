using System;
using System.Collections.Generic;
/*
 * Pathfinder
 *
 * Purpose: Generate paths from one point to another on the tile map.
 * Will initially use breadth first search.
 */
public class Pathfinder
{
    // 2D array of ints where value represents wall or floor.
    private int[,] _mapArr;
    
    // Number of floor tiles, counted at construction.
    private int _mapSize; 
    
    // Maps x, y coordinates to indices in the array used to mark tiles as visited.
    private Dictionary<IntVec2, int> _coordToIndex;

    public Pathfinder(int[,] mapArr)
    {
        _mapArr = mapArr;
        _coordToIndex = new Dictionary<IntVec2, int>();
        _mapSize = CountFloorTiles();
    }

    private int CountFloorTiles()
    {
        int acc = 0;
        
        for (int y = 0; y < Tree.DungeonHeight; y++)
        {
            for (int x = 0; x < Tree.DungeonWidth; x++)
            {
                if (_mapArr[x, y] == Tree.TileFlagFloor)
                {
                    acc++;
                    _coordToIndex.Add(new IntVec2(x, y), acc);
                    Console.Error.WriteLine("Added vector containing " + x + ", " + y + " to _coordToIndex.");
                }
            }
        }

        return acc;
    }

    public Queue<IntVec2> ShortestPath(int startX, int startY, int destX, int destY)
    {
        // If you follow from the end of the path to the start,
        // this dictionary contains the route backwards.
        Dictionary<IntVec2, IntVec2> prev = BFS(new IntVec2(startX, startY));
        
        // Set up vectors from input given
        IntVec2 start = new IntVec2(startX, startY);
        IntVec2 end = new IntVec2(destX, destY);

        // The path is given as a queue where the first node should be the first step between start and dest.
        Queue<IntVec2> path = new Queue<IntVec2>();

        IntVec2 at = end;

        while (prev.ContainsKey(at))
        {
            path.Enqueue(at);
            at = prev[at];
        }

        if (path.Peek() == start)
            return path;
        
        return null;
    }

    private Dictionary<IntVec2, IntVec2> BFS(IntVec2 start)
    {
        // The dictionary _coordToIndex can convert an (x, y) coord given as a DIYVector2 
        // into a coordinate. It converts from coordinates somewhere between (0, 0) and (79, 39) to coordinates
        // somewhere between (0, 3199), if I understand it correctly.
        bool[] visited = new bool[_mapSize];
        Console.WriteLine("Visited: " + visited.Length + ", _coordToIndex: " + _coordToIndex.Count);
        
        // Queue will store tiles to visit.
        Queue<IntVec2> q = new Queue<IntVec2>();
        
        q.Enqueue(start);
        
        // For each tile, the previous tile in the path is stored.
        // If the path is 3 -> 4 -> 5 -> 6, path reconstruction goes something like:
        // - put in the key 6 to get the value 5
        // - put in the key 5 to get the value 4
        // - put in the key 4 to get the value 3.
        Dictionary<IntVec2, IntVec2> prev = new Dictionary<IntVec2, IntVec2>();

        IntVec2 curr;
        
        while (q.Count > 0)
        {
            curr = q.Dequeue();
            // visit curr
            //Console.WriteLine("Going to try to access visited [ " + _coordToIndex[curr] + " ]. ");
            //Console.WriteLine("Does _coordToIndex have it: " + _coordToIndex.ContainsKey(curr));
            //Console.WriteLine("Does visited have that many indices? its size is: " + visited.Length);
            visited[_coordToIndex[curr]] = true;
            
            // iterate through the neighbours, adding any unmarked to the queue
            for (int y = curr.Y - 1; y <= (curr.Y + 1) % Tree.DungeonHeight; y++)
            {
                for (int x = curr.X - 1; x <= (curr.X + 1) % Tree.DungeonWidth; x++)
                {
                    // Skip an x, y which exceeds the bounds.
                    if (x > Tree.DungeonWidth - 1 || x < 0)
                        continue;

                    if (y > Tree.DungeonHeight - 1 || y < 0)
                        continue;

                    // Do not mark self when checking neighbours.
                    if (curr.X == x && curr.Y == y)
                        continue;
                    
                    // We have checked if _mapArr[x, y] exists and is not curr, but it remains
                    // to make sure it is actually a floor tile.
                    if (_mapArr[x, y] == Tree.TileFlagFloor)
                    {
                        Console.WriteLine("Creating a neighbour vector for " + x + ", " + y);
                        IntVec2 neighbour = new IntVec2(x, y);
                        if (!_coordToIndex.ContainsKey(neighbour))
                        {
                            Console.WriteLine("_coordToIndex did not contain " + neighbour.X + ", " + neighbour.Y);
                        }
                        else
                        {
                            Console.WriteLine("Going to try to access visited [ " + _coordToIndex[neighbour] + " ]. ");
                            Console.WriteLine("Does _coordToIndex have it: " + _coordToIndex.ContainsKey(neighbour));
                            Console.WriteLine("Does visited have that many indices? its size is: " + visited.Length);
                            if (!visited[_coordToIndex[neighbour]])
                            {
                                Console.WriteLine("Found that " + neighbour.X + ", " + neighbour.Y +
                                                  "was not visited, marking.");
                                q.Enqueue(neighbour);
                                visited[_coordToIndex[neighbour]] = true;
                                prev.TryAdd(neighbour, curr);
                            }
                        }
                    }
                }
            }
        }
        return prev;
    }

    public static void Main(string[] args)
    {
        // create a map
        Tree tree = new Tree();
        int[,] map = tree.GenerateMap();
        // draw it to the screen
        tree.PrintMapArr(map);
        // compute the shortest path between two random points
        Pathfinder pf = new Pathfinder(map);
        int startX = Convert.ToInt32(Console.ReadLine());
        int startY = Convert.ToInt32(Console.ReadLine());
        int destX = Convert.ToInt32(Console.ReadLine());
        int destY = Convert.ToInt32(Console.ReadLine());
        Queue<IntVec2> path = pf.ShortestPath(startX, startY, destX, destY);
        foreach (IntVec2 v in path)
            Console.WriteLine("X: " + v.X + ", Y: " + v.Y);
    }
}

public class IntVec2
{
    public int X;
    public int Y;

    public IntVec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(IntVec2 v1, IntVec2 v2)
    {
        return v1.X == v2.X && v1.Y == v2.Y;
    }

    public static bool operator !=(IntVec2 v1, IntVec2 v2)
    {
        return !(v1 == v2);
    }

    public override bool Equals(object obj)
    {
        if (obj is IntVec2 other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
}

