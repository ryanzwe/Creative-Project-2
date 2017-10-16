using System;
using Enemy;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Object = System.Object;


public class EnemyAttack : State<EnemyAI>
{
    private static EnemyAttack instance;
    private GameObject ownerGB;
    public GameObject target;
    private NavMeshAgent navAgent;
    private EnemyAttackTimer enemyTimers;
    public float attackRange = 5.5f;
    public float AttackDamage = 20f;
    public int AttackSpeedInMS = 1300;
    private bool preparingAttack;

    public EnemyAttack(GameObject ownerGB, NavMeshAgent navAgent, EnemyAttackTimer enemyTimers, GameObject target)// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null) return;
        this.ownerGB = ownerGB;
        this.navAgent = navAgent;
       // instance = this;
        this.enemyTimers = enemyTimers;
        this.target = target;
    }

    //public static EnemyAttack Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            new EnemyAttack(null, null, null,null);// IF this state doesn't exist the first time it's called, then create it
    //            Debug.Log("EnemyAttack instance call was null, creating new instance!, I probably forgot to make one in EnemyAI");
    //        }
    //        return instance;
    //    }
    //}
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

        float dist = (target.transform.position - navAgent.transform.position).magnitude;
        if (dist < attackRange)
        {
            if (!enemyTimers.isActiveAndEnabled)
                enemyTimers.enabled = true;
            if (enemyTimers.CanAttack)
            {
                Attack();
                enemyTimers.enabled = false;

            }
        }
        if (dist > attackRange)
            {
                preparingAttack = false;
                navAgent.isStopped = false;
                Owner.stateMachine.ChangeState(Owner.findTarget);
            }
    }

    public override void DebugState(EnemyAI Owner)
    {
        // Not needed yet
    }




    private void Attack()
    {
        Debug.Log("Hit!");
        PlayerController.Instance.Health -= AttackDamage;
    }
}

