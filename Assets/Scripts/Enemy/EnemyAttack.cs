using System;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Object = System.Object;


public class EnemyAttack : State<EnemyAI>
{
    private static EnemyAttack instance;


    private EnemyAI Owner;

    public EnemyAttack()// When constructing this state, set the instance to this if there has already been one created
    {
        if (instance != null) return;
        instance = this;
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
       // Debug.Log("Entering EnemyAttack");
        this.Owner = Owner;

    }

    public override void ExitState(EnemyAI Owner)
    {
       //Debug.Log("Exiting EnemyAttack");
    }

    public override void UpdateState(EnemyAI Owner)
    {

        float dist = (Owner.Target.transform.position - Owner.agent.transform.position).magnitude;
        if (dist < Owner.attackRange)
        {
            if (!Owner.enemyTimers.isActiveAndEnabled)
                Owner.enemyTimers.enabled = true;
            if (Owner.enemyTimers.CanAttack)
            {
                Attack();
                Owner.enemyTimers.enabled = false;
            }
        }
        if (dist > Owner.attackRange)
            {
                Owner.preparingAttack = false;
                Owner.agent.isStopped = false;
                Owner.stateMachine.ChangeState(Owner.findTarget);
            }
    }

    public override void DebugState(EnemyAI Owner)
    {
        // Not needed yet
    }




    private void Attack()
    {
     //   Debug.Log("Hit!");
        PlayerController.Instance.Health -= Owner.AttackDamage;
        Owner.GetComponent<Animator>().SetTrigger("Attack");
        
    }
}

