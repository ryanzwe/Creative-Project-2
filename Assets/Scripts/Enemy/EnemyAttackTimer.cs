using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTimer : MonoBehaviour
{

    public float AttackSpeed = 3;
    public bool CanAttack;
    private float attackCounter;
    private float timeTillAttack;
    private float DecrementAmt;
    private void Start()
    {
        attackCounter = timeTillAttack = AttackSpeed;
        DecrementAmt = (attackCounter / AttackSpeed);
    }

    private void OnEnable()
    {
        GameController.Instance.OnSecondChange += ACount;
    }
    private void OnDisable()
    {
        timeTillAttack = 3;
        CanAttack = false;
        GameController.Instance.OnSecondChange -= ACount;
    }

    private void ACount()
    {
        if (timeTillAttack > 0)
        { // created timeTillAttack instead of attackCounter to prevent it becoming exponentially smaller, instead of set amt
            timeTillAttack -= DecrementAmt;
            //Debug.Log("Atk in: " + timeTillAttack);
            return;
        }
        CanAttack = true;
    }
}
