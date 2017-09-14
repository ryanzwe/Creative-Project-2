using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int Damage = 20;
    public float FireRate = 30f;
    public float KnockBack = 5f;
    public float Range = 20f;
    public float ReloadSpeed = 2f;
    //Reloading & ammo
    public int ClipSize;
    public int clip;
    private bool reloading = false;
    private float nextFire;
    [Header("Particles")]
    public ParticleSystem MuzzleFlash;
    [Header("Audio")]
    public AudioClip[] ShootingSounds;
    public AudioSource audio;

    private Camera mainC;
    public Animator anim;
    private int bulletsShot;
    private void Start()
    {
        clip = ClipSize;
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void OnEnable()
    {
        audio.Stop();
        reloading = false;
        anim.SetBool("Reloading", false);
    }
    private void Update()
    {// If reloading, do nothing, if out of ammo, reload and exit loop
        if (reloading) return;
        if (clip <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetMouseButton(0) && nextFire < Time.time)
        {
            nextFire = Time.time + (1 / FireRate); // Allowing the player to shoot at the desired bullets per second
            Fire();// Starting shooting 
            Audio();// Play gun audio 
            CharController.ToggleCursour(true);
        }
    }

    private void Fire()
    {
        clip--;
        MuzzleFlash.Play();

        RaycastHit hit;
        Debug.DrawRay(mainC.transform.position, mainC.transform.forward * Range, Color.red,1);
        if (Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, Range))
        {
            bulletsShot++;
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyController>().Damage(Damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * KnockBack);
            }
            GameObject temp = GameController.Instance.HitPSPooled[bulletsShot % GameController.Instance.ImpactPSPool].gameObject;
            temp.transform.SetPositionAndRotation(hit.point,Quaternion.LookRotation(hit.normal));
            temp.SetActive(true);
        }
       
    }

    private void Audio()
    {
        int r = Random.Range(0, ShootingSounds.Length);
        audio.clip = ShootingSounds[r];
        audio.Play();

    }

    private IEnumerator Reload()
    {// Starting reload
        reloading = true;
        anim.SetBool("Reloading",reloading);
        yield return new WaitForSeconds(ReloadSpeed- 0.25f);// The nimator has a .25f transition delay
        // ending reload, animator needs to finish first to prevent player from shooting early
        anim.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);// adding this to prevent the player shooting during transition
        reloading = false;
        clip = ClipSize;
    }
}
