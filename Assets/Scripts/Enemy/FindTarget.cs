using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class FindTarget : State<EnemyAI>
{
    private static FindTarget instance;

    /* bug:
     *  Disabled the instance as two of them can't share the same movement, maybe change to a  li st?
     *  So that the new gameobjects get added to a list in this of gameobjects that get their target, and when they switch state to remove them
     */
    public FindTarget()// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public static FindTarget Instance
    {
        get
        {
            if (instance == null)
            {
                new FindTarget();// IF this state doesn't exist the first time it's called, then create it
            }
            return instance;
        }
    }

    public override void EnterState(EnemyAI Owner)
    {
        // Debug.Log("Entering  FindTarget");
        // DebugState(Owner);
    }

    public override void ExitState(EnemyAI Owner)
    {
        //Debug.Log("Exiting FindTarget");
    }

    public override void UpdateState(EnemyAI Owner)
    {
        {
            if (Owner.agent.destination != Owner.Target.transform.position && Owner.agent.enabled)
            {
                Owner.agent.destination = Owner.Target.transform.position;
                // Debug.Log("N: " + Owner.gameObject.name);
            }
            float dist = (Owner.Target.transform.position - Owner.agent.transform.position).magnitude;
            if (dist < Owner.attackRange)
            {
                Owner.agent.isStopped = true;
                Owner.stateMachine.ChangeState(Owner.enemyAttack);
            }
        }
    }
    public override void DebugState(EnemyAI Owner)
    {

    }
}
