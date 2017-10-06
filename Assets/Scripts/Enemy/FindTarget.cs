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

    private NavMeshAgent[] navAgent;

    public FindTarget(GameObject ownerGB, float targetRadius, string targetTag, NavMeshAgent[] navAgent)// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null) return;
        this.ownerGB = ownerGB;
        this.targetRadius = targetRadius;
        this.targetTag = targetTag;
        this.navAgent = navAgent;
        instance = this;
    }

    public static FindTarget Instance
    {
        get
        {
            if (instance == null)
            {
                new FindTarget(null, 0f, "", null);// IF this state doesn't exist the first time it's called, then create it
                Debug.Log("FindTarget instance call was null, creating new instance!");
            }
            return instance;
        }
    }

    public override void EnterState(EnemyAI Owner)
    {
        Debug.Log("Entering  FindTarget");
    }

    public override void ExitState(EnemyAI Owner)
    {
        Debug.Log("Exiting FindTarget");
    }

    public override void UpdateState(EnemyAI Owner)
    {
        Collider[] targs = Physics.OverlapSphere(this.ownerGB.transform.position, this.targetRadius);
        if (foundTarget == null)
        {
            for (int i = 0; i < targs.Length; i++)
            {
                if (targs[i].CompareTag(this.targetTag))
                {
                    foundTarget = targs[i].gameObject;
                }
                if (foundTarget != null) break;
            }
        }
        else
        {
            foreach (NavMeshAgent nav in navAgent)
            {
                if (nav.destination != foundTarget.transform.position && nav.enabled) nav.destination = foundTarget.transform.position;
                float dist = (foundTarget.transform.position - nav.transform.position).magnitude;
                if (dist < EnemyAttack.Instance.attackRange)
                {
                    EnemyAttack.Instance.target = foundTarget;
                    nav.isStopped = true;
                    Owner.stateMachine.ChangeState(EnemyAttack.Instance);
                }
            }
        }
    }
    public override void DebugState(EnemyAI Owner)
    {
        Debug.Log(
            instance.ToString() + " " +
            ownerGB + " " +
            targetRadius + " " +
            targetTag + " " +
            foundTarget);
    }
}
