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

    public int MaxClipsAllowed = 5;
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

    public int clipAmount = 3;// how many clips to have  -- CLIP QTY ZONE

    public int ClipAmount
    {
        get { return clipAmount; }
        set
        {
            if (value > MaxClipsAllowed)
            {
                Debug.Log("Max Ammo");
                return;
            }
            clipAmount = value;
            if (CharController.Instance.Cur != this) return; // Don't update the UI on picking up ammo if on a diff gun
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
    public CharController Char;
    private void Start()
    {
        initialized = true;
        CurrentClip = ClipSize;
        mainC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    private void OnEnable()
    {
        audio.Stop();
        reloading = false;
        Char.UpdateCurrentWeapon(this);// tell the charcontroller that this is the new gun, to update for sprinting
        if (initialized)
        {
            CurrentClip = currentClip; // Used to trigger the properties set function and update UI
            ClipAmount = clipAmount;
        }
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
        else if (Input.GetButtonDown("Fire1") && clipAmount == 0 && currentClip == 0)
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
            // Spawning a pooled particle system at the location 
            GameObject temp = GameController.Instance.HitPSPooled[bulletsShot % GameController.Instance.ImpactPSPool].gameObject;
            // If they are shooting anything but the player then grab the material, and set the particlesystems material to it to appear as if it were shooting chunks
            if (!hit.transform.CompareTag("Enemy"))
            {// If the object being shot does have a material assigned to it
                if (hit.transform.gameObject.GetComponent<Renderer>().material.mainTexture != null)
                {
                    // Grabbing the texture from what was shot
                    Texture2D tex = (Texture2D)hit.transform.gameObject.GetComponentInChildren<Renderer>().material
                        .mainTexture;
                    // Grabbing the UV from where the raycast had hit 
                    Vector2 UVs = new Vector2(hit.textureCoord.x * tex.width, hit.textureCoord.y * tex.height);
                    // getting the pixels from the UV co-ords
                    Color col = tex.GetPixel(Mathf.RoundToInt(UVs.x), Mathf.RoundToInt(UVs.y));
                    // setting the particle systems materials colour to the colour data
                    temp.GetComponent<Renderer>().material.color = col;
                }
                else // If the object being shot doesn't have a texture assigned to it (meaning it's a flat material)
                    temp.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;
            }
            else
            {// If an enemy was shot, make them take damage and drop blood 
                hit.transform.GetComponent<EnemyController>().Damage(Damage);
                temp.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
            }
            // Set the position of the pooled ps to where it was hit by the raycast, and set the rotation the direction the normal was facing
            temp.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
            temp.SetActive(true);

            //if (hit.rigidbody != null)
            //{
            //    // hit.rigidbody.AddForce(-hit.normal * KnockBack);
            //}
        }
    }

    private void Audio()
    {
        // Choose a random audio clip from the audio array and play it 
        int r = Random.Range(0, ShootingSounds.Length);
        audio.clip = ShootingSounds[r];
        audio.Play();

    }

    private IEnumerator Reload()
    {// Starting reload
        reloading = true;
        anim.SetTrigger("Reloading");  // Play thee reloading animation
        // play the sound and delay for the reload speed take the animation transition delay, then reload and set the clip size again
        audio.PlayOneShot(ReloadSound);
        yield return new WaitForSeconds(ReloadSpeed - 0.25f);// The reload anim has a .25f transition delay, alows player to shoot when anim just finishes, instead of waiting
        reloading = false;
        // Take away ammo and set the clip back to how many bullets it's supposed to have 
        ClipAmount--;
        CurrentClip = ClipSize;

    }


}
