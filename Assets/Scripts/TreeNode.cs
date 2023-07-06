
public class TreeNode
{
    // ====================================================
    // Data Members
    // ====================================================
    private TreeNode _parent;
    private TreeNode[] _children = new TreeNode[2]; // [leftChild, rightChild]
    
    public Area Data;

    // ====================================================
    // Constructor
    // ====================================================
    public TreeNode(TreeNode parentNode)
    {
        _parent = parentNode;
    }

    // ====================================================
    // Accessor & Mutator Methods
    // ====================================================
    public void SetParent(TreeNode newParent)
    {
        _parent = newParent;
    }

    public TreeNode GetParent()
    {
        return _parent;
    }

    public void SetLeftChild(TreeNode newLChild)
    {
        _children[0] = newLChild;
    }

    public TreeNode GetLeftChild()
    {
        return _children[0];
    }

    public void SetRightChild(TreeNode newRChild)
    {
        _children[1] = newRChild;
    }

    public TreeNode GetRightChild()
    {
        return _children[1];
    }
}
