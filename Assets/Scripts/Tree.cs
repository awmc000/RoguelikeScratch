using System;
using System.Collections.Generic;
using Random = System.Random;

public class Tree
{
    private TreeNode _root;
    private List<TreeNode> _leafNodes;

    public const int DungeonWidth   = 80;
    public const int DungeonHeight  = 40;
    public const int SplitMargin    = 1;
    public const float SplitMinRatio= 0.35f;
    public const float SplitMaxRatio= 0.65f;

    private const int RoomMinWidth  = 3;
    private const int RoomMinHeight = 3;

    private Random _random = new Random();
    
    public Tree()
    {
        _root = new TreeNode(null);
        _root.Data = new Area(0, 0, DungeonWidth, DungeonHeight);
        _leafNodes = new List<TreeNode>();
        _leafNodes.Add(_root);
    }

    public TreeNode GetRoot()
    {
        return _root;
    }
    
    public void SetRoot(TreeNode root)
    {
        _root = root;
    }

    public List<TreeNode> GetLeafNodes()
    {
        return _leafNodes;
    }

    // Split each leaf. Make the leafs children the new leafs.
    public void SplitAll()
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

    public void Split(TreeNode node)
    {
        TreeNode newLChild = new TreeNode(node);
        TreeNode newRChild = new TreeNode(node);
        
        node.SetLeftChild(newLChild);
        node.SetRightChild(newRChild);

        // choose a random direction : horizontal or vertical splitting
        bool splitVertically = _random.Next(2) == 1;
        if (splitVertically)
        {
            HalveVertically(node, newLChild, newRChild);
        }
        else
        {
            HalveHorizontally(node, newLChild, newRChild);
        }   
    }

    public void PrintMap()
    {
        int[,] mapArr = new int[DungeonWidth, DungeonHeight];

        Console.WriteLine("Size of arr: " + mapArr.Length);
        int numLeafNodes = _leafNodes.Count;

        TreeNode currentNode;
        Area currentArea;
        
        for (int i = 0; i < numLeafNodes; i++)
        {
            currentNode = _leafNodes[i];
            currentArea = currentNode.Data;

            for (int y = 0; y < currentArea.H; y++)
            {
                for (int x = 0; x < currentArea.W; x++)
                {
                    mapArr[currentArea.X + x, currentArea.Y + y] = i;
                    Console.WriteLine("Set {0}, {1} to {2}", x, y, i);
                }
            }
        }

        Console.WriteLine(mapArr[2, 2]);
        
        for (int y = 0; y < DungeonHeight; y++)
        {
            for (int x = 0; x < DungeonWidth; x++)
            {
                int v = mapArr[x, y];
                Console.Write(v);
            }
            Console.WriteLine();
        }
    }
    
    // Split `node` into exact halves along a vertical line. No random generation of room dimensions.
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
    
    private void SplitVertically(TreeNode node, TreeNode newLChild, TreeNode newRChild)
    {
        // choose a random position (x for vertical)
        int minSplit = node.Data.X + SplitMargin;
        Console.WriteLine("minSplit: " + minSplit);
        int maxSplit = node.Data.X + node.Data.W - SplitMargin;
        Console.WriteLine("maxSplit: " + maxSplit);

        int splitPosition = (minSplit + ((maxSplit - minSplit) / 2));
        while (((double)splitPosition < SplitMinRatio) ||
               ((double)splitPosition / (node.Data.W - SplitMargin) > SplitMaxRatio))
        {
            splitPosition = _random.Next(RoomMinWidth, node.Data.W - SplitMargin);
        }

        Console.WriteLine("Split position: " + splitPosition);
        int maxLeftWidth = splitPosition - SplitMargin - 1;
        Console.WriteLine("maxLeftWidth: " + maxLeftWidth);
        int maxRightWidth = maxSplit - splitPosition;
        Console.WriteLine("maxRightWidth: " + maxRightWidth);



        int leftWidth = 0;
        Console.WriteLine("Computing left width");
        while (((double)leftWidth / maxLeftWidth < SplitMinRatio) ||
               (((double)leftWidth / maxRightWidth) > SplitMaxRatio))
        {
            Console.WriteLine("About to get random between " + RoomMinWidth + " and " + maxLeftWidth);
            if (RoomMinWidth > maxLeftWidth)
            {
                Console.WriteLine("Can't make this room");
                return;
            }

            leftWidth = _random.Next(RoomMinWidth, maxLeftWidth);
            Console.WriteLine("leftWidth random genned to " + leftWidth);
        }

        int rightWidth = _random.Next(RoomMinWidth, maxRightWidth);
        Console.WriteLine("Computing right width");
        while (((double)rightWidth / maxRightWidth < SplitMinRatio) ||
               (((double)rightWidth / maxRightWidth) > SplitMaxRatio))
        {
            Console.WriteLine("About to get random between " + RoomMinWidth + " and " + maxRightWidth);
            if (RoomMinWidth > maxRightWidth)
            {
                Console.WriteLine("Can't make this room");
            }

            rightWidth = _random.Next(RoomMinWidth, maxRightWidth);
            Console.WriteLine("rightWidth random genned to " + rightWidth);
        }
    }

    private static void PrintGoodStuff(List<TreeNode> nodeList)
    {
        Console.WriteLine("========= Current Leaf Nodes ==============");
        foreach (TreeNode node in nodeList)
        {
            Console.Write("x = " + node.Data.X);
            Console.Write("y = " + node.Data.Y);
            Console.Write("w = " + node.Data.W);
            Console.WriteLine("h = " + node.Data.H);
        }
    }
    public static void Main(string[] args)
    {
        Tree tree = new Tree();
        var li1 = tree.GetLeafNodes();
        PrintGoodStuff(li1);
        
        tree.SplitAll();
        var li2 = tree.GetLeafNodes();
        PrintGoodStuff(li2);
        
        tree.SplitAll();
        var li3 = tree.GetLeafNodes();
        PrintGoodStuff(li3);
        
        tree.SplitAll();
        var li4 = tree.GetLeafNodes();
        PrintGoodStuff(li4);
        
        /*
        tree.SplitAll();
        var li5 = tree.GetLeafNodes();
        PrintGoodStuff(li5);
        */
        tree.PrintMap();
    }
}
