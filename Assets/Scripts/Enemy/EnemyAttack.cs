using System;
using Enemy;
using NUnit.Framework.Constraints;
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
    public float AttackDamage = 20f;
    public int AttackSpeedInMS = 1300;
    private bool preparingAttack;
    private System.Timers.Timer[] attackTimer;
    

    public EnemyAttack(GameObject ownerGB, NavMeshAgent[] navAgent)// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null) return;
        this.ownerGB = ownerGB;
        this.navAgent = navAgent;
        instance = this;
        TimerSetup();
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
        
    }

    public override void ExitState(EnemyAI Owner)
    {
        Debug.Log("Exiting EnemyAttack");
    }

    public override void UpdateState(EnemyAI Owner)
    {
        for (int i = 0; i < navAgent.Length; i++)
        {
            float dist = (target.transform.position - navAgent[i].transform.position).magnitude;
            if (dist < attackRange)
            {
                attackTimer[i].Start();
            }
            if (dist > attackRange)
            {
                preparingAttack = false;
                attackTimer[i].Stop();
                navAgent[i].isStopped = false;
                Owner.stateMachine.ChangeState(FindTarget.Instance);
            }
        }
        //foreach (NavMeshAgent agent in navAgent)
        //{
        //    float dist = (target.transform.position - agent.transform.position ).magnitude;
        //    if (dist < attackRange)
        //    {
        //        preparingAttack = true;
        //        attackTimer.Start();
        //    }
        //    if (preparingAttack && dist > attackRange)
        //    {
        //        preparingAttack = false;
        //        attackTimer.Stop();
        //        agent.isStopped = false;
        //        Owner.stateMachine.ChangeState(FindTarget.Instance);
        //    }
        //}
    }

    public override void DebugState(EnemyAI Owner)
    {
        // Not needed yet
    }

    private void TimerSetup()
    {
        attackTimer = new System.Timers.Timer[navAgent.Length];
        Debug.Log(attackTimer.Length);
        for (int i = 0; i < attackTimer.Length; i++)
        {
            attackTimer[i].Interval = AttackSpeedInMS;
            attackTimer[i].AutoReset = false;
            attackTimer[i].Elapsed += Attack;
            
        }
    }



    private void Attack(Object obj, EventArgs e)
    {
        Debug.Log("Hit!");
        PlayerController.Instance.Health -= AttackDamage;
    }   
}

