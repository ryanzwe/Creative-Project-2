using Enemy;
using UnityEngine;

public class FirstState : State<EnemyAI>
{
    private static FirstState instance;

    private FirstState()
    {
        if (instance != null) return;
        instance = this;
    }

    public static FirstState Instance
    {
        get
        {// when calling the instance
            if (instance == null) new FirstState();// IF this state doesn't exist the first time it's called, then create it
            return instance;
        }
    }
    public override void EnterState(EnemyAI Owner)// Called once
    {
       Debug.Log("Entering First State");
    }

    public override void ExitState(EnemyAI Owner)// called once 
    {
        Debug.Log("Exiting First State");
    }

    public override void UpdateState(EnemyAI Owner)
    {   
        Debug.Log("Updating First State");
        
        //if (Owner.switchState)
        //{
        //    Owner.stateMachine.ChangeState(SecondState.Instance);
        //}
    }

    public override void DebugState(EnemyAI Owner)
    {
        
    }
}


