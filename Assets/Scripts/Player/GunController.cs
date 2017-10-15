using System.Collections;
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
    public int ClipSize;// How many bullets there can be  -- CLIP ZONE
    private int currentClip;
    public int CurrentClip
    {
        get { return currentClip; }
        set
        {
            currentClip = value;
            GameController.Instance.ui.AmmoCount.text = currentClip.ToString();
        }
    }

    private int clipAmount = 3;// how many clips to have  -- CLIP QTY ZONE

    public int ClipAmount
    {
        get { return clipAmount; }
        set
        {
            clipAmount = value;
            GameController.Instance.ui.ClipCount.text = clipAmount.ToString();
        }
    }

    private bool reloading;
    private float nextFire;
    [Header("Particles")]
    public ParticleSystem MuzzleFlash;
    [Header("Audio")]
    public AudioClip[] ShootingSounds;
    public AudioClip ReloadSound;
    public AudioClip DrySound;
    public AudioSource audio;
    //Extra
    private Camera mainC;
    public Animator anim;
    private int bulletsShot;
    private bool initialized = false;
    private void Start()
    {
        initialized = true;
        CurrentClip = ClipSize;
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void OnEnable()
    {
        audio.Stop();
        reloading = false;
        if (initialized)
            CurrentClip = currentClip; // Used to trigger the properties set function and update UI
    }
    private void Update()
    {// If reloading, do nothing, if out of ammo, reload and exit loop
        if (reloading) return;
        if (currentClip <= 0 && clipAmount > 0 || Input.GetKeyDown(KeyCode.R) && clipAmount > 0)
        {// If the player has no ammo, and clips remaining then reload 
            StartCoroutine(Reload());
            return;
        }                                                      // >= so we can shoot when no spare clips left
        if (Input.GetButton("Fire1") && nextFire < Time.time && clipAmount >= 0 && currentClip > 0)
        {// If the player DOES have ammo, and is clicking while above the fire rate then shoot 
            nextFire = Time.time + (1 / FireRate); // Allowing the player to shoot at the desired bullets per second
            Fire();// Starting shooting 
            Audio();// Play gun audio 
            CharController.ToggleCursour(true);
            //StartCoroutine(AnimationFlick("Shooting", 0.001f));
            anim.SetTrigger("Shooting");
        }
        else if(Input.GetButtonDown("Fire1") && clipAmount == 0 && currentClip == 0)
             audio.PlayOneShot(DrySound);
        // If the gun has no ammo left in the clip, no clips remaining, and can't reload then play the dry sound 
    }

    private void Fire()
    {
        CurrentClip--;
        MuzzleFlash.Play();
        RaycastHit hit;
        Debug.DrawRay(mainC.transform.position, mainC.transform.forward * Range, Color.red, 1);
        if (Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, Range))
        {
            bulletsShot++;
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyController>().Damage(Damage);
            }
            if (hit.rigidbody != null)
            {
               // hit.rigidbody.AddForce(-hit.normal * KnockBack);
            }
            // spawning a particle system where it hits 
            GameObject temp = GameController.Instance.HitPSPooled[bulletsShot % GameController.Instance.ImpactPSPool].gameObject;
            if (hit.transform.GetComponent<Renderer>() != null)
            {
                temp.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;
                if (hit.transform.CompareTag("Enemy"))
                { // If shooting the enemy, make the material red so it looks like blood 
                    temp.GetComponent<Renderer>().material.color = new Color(1f,0f,0f);
                }// If not change the material to what it's shooting, to make it look better 
            }
            temp.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
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
        //if(clipAmount >0)
            ClipAmount--;
        anim.SetTrigger("Reloading");
        audio.PlayOneShot(ReloadSound);
        yield return new WaitForSeconds(ReloadSpeed - 0.25f);// The reload anim has a .25f transition delay, alows player to shoot when anim just finishes, instead of waiting
        reloading = false;
        CurrentClip = ClipSize;

    }
    private IEnumerator AnimationFlick(string Parameter, float delay, bool NotReverse = true)
    {
        anim.SetBool(Parameter, NotReverse);
        yield return new WaitForSeconds(delay);
        anim.SetBool(Parameter, !NotReverse);

    }

}
