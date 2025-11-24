using System.Collections.Generic;
using UnityEngine;

public class Selector : BtNode
{
    // 노드 중 하나라도 성공 (우선순위 판단) 실패해야 다음노드
    
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
public class Selector_Random : BtNode
{
    private BtNode _chosenNode;

    public Selector_Random(List<BtNode> children) : base(children) { }

    public override NodeState Evaluate()
    {
        // 이 노드의 이전 상태가 'Running'이 아니었거나, 선택된 자식 노드가 없다면 새로운 자식을 선택합니다.
        if (State != NodeState.Running || _chosenNode == null)
        {
            int randomIndex = Random.Range(0, Children.Count);
            _chosenNode = Children[randomIndex];
        }

        if (_chosenNode == null)
        {
            return NodeState.Failure;
        }

        // 선택된 자식 노드를 평가합니다.
        NodeState result = _chosenNode.Evaluate();

        // 자식 노드가 Running 상태가 아니면(Success 또는 Failure), 다음 평가 때 새로운 선택을 할 수 있도록 초기화합니다.
        if (result != NodeState.Running)
        {
            _chosenNode = null;
        }

        State = result;
        return State;
    }
}

public class Sequence : BtNode
{
    // 노드가 모두 성공 (연속 되는 행동) 성공해야 다음노드
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