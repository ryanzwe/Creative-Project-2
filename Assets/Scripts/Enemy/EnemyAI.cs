using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int health = 100;
    public int ScoreValue = 50;
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
        stateMachine.Update();
        CurrentStateString = stateMachine.CurrentState.ToString();
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            if (Random.value > 0.5f)
            {
                GameObject gb = Instantiate(GameController.Instance.Log, transform.position + (Vector3.up * 0.8f), Quaternion.identity);
               // gb.GetComponent<Rigidbody>().AddForce(Vector3.up * 15f);
            }
            GameController.Instance.Score += ScoreValue;
            EnemyPooling.Instance.Deactivate(this.gameObject);

        }
    }
}
