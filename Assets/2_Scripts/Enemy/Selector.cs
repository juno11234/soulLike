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
            _chosenNode = Children[randomIndex]; // 디버깅을 위해 강제로 1번 자식(strafe)을 선택
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
public class Sequence_Memory : BtNode
{
    private int _lastRunningChildIndex = 0;

    public Sequence_Memory(List<BtNode> children) : base(children) { }

    public override NodeState Evaluate()
    {
        // 이 시퀀스가 이전에 실행 중(Running) 상태가 아니었다면, 처음부터 시작합니다.
        if (State != NodeState.Running)
        {
            _lastRunningChildIndex = 0;
        }

        for (int i = _lastRunningChildIndex; i < Children.Count; i++)
        {
            NodeState childState = Children[i].Evaluate();

            switch (childState)
            {
                case NodeState.Failure:
                    // 자식 중 하나라도 실패하면, 이 시퀀스 전체가 실패입니다.
                    _lastRunningChildIndex = 0; // 상태 초기화
                    State = NodeState.Failure;
                    return State;

                case NodeState.Success:
                    // 자식이 성공하면, 루프를 계속 진행하여 다음 자식을 평가합니다.
                    continue;

                case NodeState.Running:
                    // 자식이 아직 실행 중이라면, 어디까지 실행했는지 기억하고 이 시퀀스 전체도 Running 상태가 됩니다.
                    _lastRunningChildIndex = i;
                    State = NodeState.Running;
                    return State;
            }
        }

        // 모든 자식들이 성공적으로 실행 완료되었습니다.
        _lastRunningChildIndex = 0; // 상태 초기화
        State = NodeState.Success;
        return State;
    }
}