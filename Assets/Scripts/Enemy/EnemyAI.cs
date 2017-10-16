using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public StateMachine<EnemyAI> stateMachine { get; set; }
    public string CurrentStateString = string.Empty;
    private NavMeshAgent agent;
    private EnemyAttackTimer enemyTimers;
    private GameObject player;

    public float TargetRange;
    public EnemyAttack enemyAttack;
    public FindTarget findTarget;

    private void Start()
    {
        player = GameObject.Find("Player");
        agent = transform.GetComponent<NavMeshAgent>();
        enemyTimers = transform.GetComponent<EnemyAttackTimer>();
        // Creating a new Statemachine for the Ai to use, the type is an AISate, and the Owner is this instance
        stateMachine = new StateMachine<EnemyAI>(this);
        enemyAttack = new EnemyAttack(this.gameObject, this.agent, this.enemyTimers,player); // Creating the instance
        findTarget = new FindTarget(this.gameObject, TargetRange, "Player", this.agent, player);
        stateMachine.ChangeState(findTarget);
      //  Debug.Log("Owner: " + stateMachine.Owner + "State: " + stateMachine.CurrentState);
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
        CurrentStateString = stateMachine.CurrentState.ToString();
    }

}
