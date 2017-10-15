using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public bool switchState = false;
    public float gameT = 0.0f;
    public int seconds = 0;
    public StateMachine<EnemyAI> stateMachine { get; set; }
    private NavMeshAgent[] agents;
    private EnemyAttackTimer[] enemyTimers;

    public float TargetRange;

    private void Start()
    {
        int childCt = transform.childCount;
        agents = new NavMeshAgent[childCt];
        enemyTimers = new EnemyAttackTimer[childCt];
        for (int i = 0; i < childCt; i++)
        {
            agents[i] = transform.GetChild(i).GetComponent<NavMeshAgent>();
            enemyTimers[i] = transform.GetChild(i).GetComponent<EnemyAttackTimer>();
        }
        // Creating a new Statemachine for the Ai to use, the type is an AISate, and the Owner is this instance
        stateMachine = new StateMachine<EnemyAI>(this);
        stateMachine.ChangeState(new FindTarget(this.gameObject,TargetRange,"Player",this.agents));
        new EnemyAttack(this.gameObject, this.agents,this.enemyTimers); // Creating the instance
        //stateMachine.ChangeState(FirstState.Instance);
        Debug.Log("Owner: " + stateMachine.Owner + "State: " + stateMachine.CurrentState);
    }

    void Update()
    {
        //if (Time.time > gameT + 1)
        //{
        //    gameT = Time.time;
        //    seconds++;
        //}
        //if (seconds == 5)
        //{
        //    seconds = 0;
        //    switchState = !switchState;
        //}
        stateMachine.Update();
    }

}
