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

    private Camera mainC;
    private void Start()
    {
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && nextFire < Time.time)
        {
            nextFire = Time.time + (1 / FireRate);
            Fire();
            
        }
    }

    private void Fire()
    {

        RaycastHit hit;
        Debug.DrawRay(mainC.transform.position, mainC.transform.forward * Range, Color.red,1);
        if (Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, Range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy "))
            {
                //Kill
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * KnockBack);
            }
        }
        CharController.ToggleCursour(true);
    }
}
