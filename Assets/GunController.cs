using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public int Damage = 20;
    public float FireRate = 30f;
    public float KnockBack = 5f;
    public float Range = 20f;
    private float nextFire;

    private void Start()
    {

    }

    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            CharController.ToggleCursour(true);
        }
    }
}
