using System;
using System.Timers;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Object = System.Object;


class EnemyAttack : State<EnemyAI>
{
    private static EnemyAttack instance;
    private GameObject ownerGB;
    public GameObject target;
    private NavMeshAgent[] navAgent;
    public float attackRange = 5.5f;
    private bool preparingAttack;
    private System.Timers.Timer attackTimer;

    public EnemyAttack(GameObject ownerGB, NavMeshAgent[] navAgent)// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null) return;
        this.ownerGB = ownerGB;
        this.navAgent = navAgent;
        instance = this;
    }

    public static EnemyAttack Instance
    {
        get
        {
            if (instance == null)
            {
                new EnemyAttack(null, null);// IF this state doesn't exist the first time it's called, then create it
                Debug.Log("EnemyAttack instance call was null, creating new instance!");
            }
            return instance;
        }
    }
    public override void EnterState(EnemyAI Owner)
    {
        Debug.Log("Entering EnemyAttack");
        TimerSetup();
    }

    public override void ExitState(EnemyAI Owner)
    {
        Debug.Log("Exiting EnemyAttack");
    }

    public override void UpdateState(EnemyAI Owner)
    {
        foreach (NavMeshAgent agent in navAgent)
        {
            float dist = (target.transform.position - agent.transform.position ).magnitude;
            if (dist < attackRange)
            {
                preparingAttack = true;
                attackTimer.Start();
            }
            if (preparingAttack && dist > attackRange)
            {
                preparingAttack = false;
                attackTimer.Stop();
                agent.isStopped = false;
                Owner.stateMachine.ChangeState(FindTarget.Instance);
            }
        }
    }

    public override void DebugState(EnemyAI Owner)
    {
        // Not needed yet
    }

    private void TimerSetup()
    {
        attackTimer = new System.Timers.Timer();
        attackTimer.Interval = 3000;
        attackTimer.AutoReset = false;
        attackTimer.Elapsed += Attack;
    }



    private void Attack(Object obj, EventArgs e)
    {
        Debug.Log("Hit!");
    }
}

