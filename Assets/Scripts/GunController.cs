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
    public ParticleSystem MuzzleFlash;
    public ParticleSystem ImpactEffect;
    public AudioClip[] ShootingSounds;
    private AudioSource audio;
    private Camera mainC;
    private void Start()
    {
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && nextFire < Time.time)
        {
            nextFire = Time.time + (1 / FireRate);
            Fire();
            Audio();
        }
    }

    private void Fire()
    {
        MuzzleFlash.Play();
        RaycastHit hit;
        Debug.DrawRay(mainC.transform.position, mainC.transform.forward * Range, Color.red,1);
        if (Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, Range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyController>().Damage(Damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * KnockBack);
            }
        }
        CharController.ToggleCursour(true);
    }

    private void Audio()
    {
        int r = Random.Range(0, ShootingSounds.Length);
        audio.clip = ShootingSounds[r];
        audio.Play();

    }
}
