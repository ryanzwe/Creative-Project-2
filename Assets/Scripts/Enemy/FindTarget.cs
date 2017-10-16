using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class FindTarget : State<EnemyAI>
{
    private static FindTarget instance;
    private GameObject ownerGB;
    private float targetRadius;
    private string targetTag = "";
    private GameObject foundTarget = null;

    private NavMeshAgent navAgent;

    private EnemyAttack attackState;

    private FindTarget findState;
    /* bug:
     *  Disabled the instance as two of them can't share the same movement, maybe change to a  li st?
     *  So that the new gameobjects get added to a list in this of gameobjects that get their target, and when they switch state to remove them
     */
    public FindTarget(GameObject ownerGB, float targetRadius, string targetTag, NavMeshAgent navAgent,GameObject player)// When constructing this state, set the instance to this if there has already been one created
    {                            
        //if (instance != null) //
        //{ //
        //    Debug.Log("Yo my dood there's already an instance, git gud");
        //    return;
        //}
        this.ownerGB = ownerGB;
        this.targetRadius = targetRadius;
        this.targetTag = targetTag;
        this.navAgent = navAgent;
       // instance = this;
        foundTarget = player;
    }

    //public static FindTarget Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            new FindTarget(null, 0f, "", null,null);// IF this state doesn't exist the first time it's called, then create it
    //            Debug.Log("FindTarget instance call was null, creating new instance!");
    //        }
    //        return instance;
    //    }
    //}

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
       // Collider[] targs = Physics.OverlapSphere(this.ownerGB.transform.position, this.targetRadius);
        if (foundTarget == null)
        {
            //for (int i = 0; i < targs.Length; i++)
            //{
            //    if (targs[i].CompareTag(this.targetTag))
            //    {
            //        foundTarget = targs[i].gameObject;
            //    }
            //    if (foundTarget != null) break;
            //}
        }
        else
        {
            if (navAgent.destination != foundTarget.transform.position && navAgent.enabled)
            {
                navAgent.destination = foundTarget.transform.position;
               // Debug.Log("N: " + Owner.gameObject.name);
            }
            float dist = (foundTarget.transform.position - navAgent.transform.position).magnitude;
            if (dist < Owner.enemyAttack.attackRange)
            {
                Owner.enemyAttack.target = foundTarget;
                navAgent.isStopped = true;
                Owner.stateMachine.ChangeState(Owner.enemyAttack);
            }
        }
    }


    public override void DebugState(EnemyAI Owner)
    {
        Debug.Log(
            instance.ToString() + " " +
            ownerGB + " " +
            targetRadius + " " +
            foundTarget);
    }
}
