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
    public BtNode parent; // 부모
    protected List<BtNode> Children = new List<BtNode>(); // 자식

    public BtNode()
    {
        parent = null;
    }

    public BtNode(List<BtNode> children)
    {
        Attach(children);
    }

    //자식 노드 연결
    public void Attach(List<BtNode> children)
    {
        foreach (BtNode child in children)
        {
            child.parent = this;
        }
        this.Children = children;
    }
    
    //노드의 로직
    public abstract NodeState Evaluate();
}
