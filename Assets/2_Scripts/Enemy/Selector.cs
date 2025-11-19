using System.Collections.Generic;
using UnityEngine;

public class Selector : BtNode
{
    public Selector() : base()
    {
    }

    public Selector(List<BtNode> children) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
        foreach (BtNode node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    continue; // 실패하면 다음 자식 확인
                case NodeState.Success:
                    State = NodeState.Success;
                    return State;
                case NodeState.Running:
                    State = NodeState.Running;
                    return State;
            }
        }

        State = NodeState.Failure;
        return State;
    }
}

public class Sequence : BtNode
{
    public Sequence() : base()
    {
    }

    public Sequence(List<BtNode> children) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        foreach (BtNode node in Children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    State = NodeState.Failure;
                    return State; // 하나라도 실패하면 즉시 실패
                case NodeState.Success:
                    continue; // 성공하면 다음 단계 진행
                case NodeState.Running:
                    anyChildIsRunning = true;
                    continue;
            }
        }

        State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
        return State;
    }
}