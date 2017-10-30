using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int health = 100;
    public int ScoreValue = 50;
    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            if (Random.value > 0.1f)
            {
                GameObject gb = Instantiate(GameController.Instance.Log,transform.position,Quaternion.identity);
                gb.GetComponent<Rigidbody>().AddForce(Vector3.up * 15f);
            }
            GameController.Instance.Score += ScoreValue;
            EnemyPooling.Instance.Deactivate(this.gameObject);

        }
    }
}
