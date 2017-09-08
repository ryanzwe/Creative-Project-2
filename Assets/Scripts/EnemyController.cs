using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int health = 20;

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            EnemyPooling.Instance.Deactivate(this.gameObject);
        }
    }
}
