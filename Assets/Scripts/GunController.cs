using System.Collections;
using System.Collections.Generic;
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
    public int ClipAmount = 3;
    private int currentClip;
    private bool reloading = false;
    private float nextFire;
    [Header("Particles")]
    public ParticleSystem MuzzleFlash;
    [Header("Audio")]
    public AudioClip[] ShootingSounds;

    public AudioClip ReloadSound;
    public AudioSource audio;

    private Camera mainC;
    public Animator anim;
    private int bulletsShot;
    private void Start()
    {
        currentClip = ClipSize;
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void OnEnable()
    {
        audio.Stop();
        reloading = false;
    }
    private void Update()
    {// If reloading, do nothing, if out of ammo, reload and exit loop
        if (reloading) return;
        if (currentClip <= 0 && ClipAmount > 0 || Input.GetKeyDown(KeyCode.R) && ClipAmount > 0)
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
            //StartCoroutine(AnimationFlick("Shooting", 0.001f));
            anim.SetTrigger("Shooting");
        }
    }

    private void Fire()
    {
        currentClip--;
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
        ClipAmount--;
        anim.SetTrigger("Reloading");
        audio.PlayOneShot(ReloadSound);
        yield return new WaitForSeconds(ReloadSpeed - 0.25f);// The reload anim has a .25f transition delay, alows player to shoot when anim just finishes, instead of waiting
        reloading = false;
        currentClip = ClipSize;
    }
    private IEnumerator AnimationFlick(string Parameter, float delay,bool NotReverse = true)
    {
        anim.SetBool(Parameter, NotReverse);
        yield return new WaitForSeconds(delay);
        anim.SetBool(Parameter, !NotReverse);

    }
   
}
