using Enemy;
using UnityEngine;

public class SecondState : State<EnemyAI>
{
    private static SecondState instance;

    private SecondState()
    {
        if (instance != null) return;
        instance = this;
    }

    public static SecondState Instance
    {
        get
        {
            if (instance == null) new SecondState();// IF this state doesn't exist the first time it's called, then create it
            return instance;
        }
    }
    public override void EnterState(EnemyAI Owner)
    {
        Debug.Log("Entering SecondState State");
    }

    public override void ExitState(EnemyAI Owner)
    {
        Debug.Log("Exiting SecondState State");
    }

    public override void UpdateState(EnemyAI Owner)
    {
        if (!Owner.switchState)
        {
            Owner.stateMachine.ChangeState(FirstState.Instance);
        }
    }
    public override void DebugState(EnemyAI Owner)
    {

    }
}
