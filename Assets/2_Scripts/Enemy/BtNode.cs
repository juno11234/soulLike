using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Success,
    Running,
    Failure
}

public abstract class BtNode
{
    protected NodeState State;
    public BtNode parent;
    protected List<BtNode> Children = new List<BtNode>();

    public BtNode()
    {
        parent = null;
    }

    public BtNode(List<BtNode> children)
    {
        Attach(children);
    }

    public void Attach(List<BtNode> children)
    {
        foreach (BtNode child in children)
        {
            child.parent = this;
        }
        this.Children = children;
    }
    public abstract NodeState Evaluate();
}