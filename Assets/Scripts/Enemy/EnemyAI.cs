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

    public float TargetRange;

    private void Start()
    {
        agents = new NavMeshAgent[transform.childCount];
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = transform.GetChild(i).GetComponent<NavMeshAgent>();
        }
        // Creating a new Statemachine for the Ai to use, the type is an AISate, and the Owner is this instance
        stateMachine = new StateMachine<EnemyAI>(this);
        stateMachine.ChangeState(new FindTarget(this.gameObject,TargetRange,"Player",this.agents));
        new EnemyAttack(this.gameObject, this.agents); // Creating the instance
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
