using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal.Input;
/// <summary>
///  Contains Movement, Rotating, Sprinting, CurrentGun assigned from WeaponManager.cs, and management of the cursour
/// </summary>
public class CharController : MonoBehaviour
{

    // Player Movement
    private float verticalMovement;
    private float horizontalMovement;
    // Player Rotation
    private float rotX;
    private float rotY;
    private float yMinRot = -55f;
    private float yMaxRot = 80;
    private Quaternion charRot;
    private Quaternion camRot;
    public float mouseSensitivity = 5.0f;

    //Forces 
    private Rigidbody rb;
    private float moveSpeed = 3.0f;
    //private float jumpSpeed = 2f;

    private bool sprinting = false;
    // Extra
    private GameObject cam;
    public float PickupDistance = 5f;
    public Animator weaponHandlerAnim;

    public Animator LogHandlerAnim;
    // Shooting control
    [SerializeField]
    private GunController cur;

    public GunController Cur
    {
        get { return cur; }
        set
        {
            cur = value;
        }
    }

    private bool canShoot;

    private static CharController instance;
    public static CharController Instance
    {
        get { return instance; }
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        instance = this;
        cam = transform.GetChild(0).gameObject;
        camRot = cam.transform.localRotation;
        charRot = transform.localRotation;
        ToggleCursour(true);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 5);
    }

    // Update is called once per frame
    private void Update()
    {
        MouseLook();
        Inputs();
    }
    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {// If sprintine make the modifier 2, else 1 
        if (Input.GetKey(KeyCode.LeftShift) && GameController.Instance.CurrentLogCount < 2) sprinting = true;
        else sprinting = false;
        float sprintModifier = sprinting ? 2 : 1;
        // Getting the position the frame prior to moving
        Vector3 prePos = transform.position;
        verticalMovement = Input.GetAxis("Vertical") * moveSpeed * sprintModifier * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * sprintModifier * Time.deltaTime;
        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement));

        // IF the character isn't at the same spot as the last frame, and is sprinting then play the anim
        if (sprinting)
        {
            weaponHandlerAnim.SetBool("Sprinting", true);
            // Disable the gun so the player can't shoot
            cur.enabled = false;
            return;
        }
        // if not sprinting
        weaponHandlerAnim.SetBool("Sprinting", false);
        // Enable the gun so the player can shoot
        if (GameController.Instance.CurrentLogCount < 2)
            cur.enabled = true;
    }

    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursour(false);
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;// Draw a ray forwards 
            Debug.DrawRay(cam.transform.position, cam.transform.forward * PickupDistance, Color.red, 1);
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, PickupDistance))
            {
                if (hit.collider.CompareTag("Log") && GameController.Instance.CurrentLogCount < 2)
                {
                    Destroy(hit.collider.gameObject);
                    GameController.Instance.CurrentLogCount++;// adding one to the playes log count, for game completion
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Extra);// as this is an extra item, play this sound 
                }
                else if (hit.collider.CompareTag("LogBase"))
                { // taking away the players logs and placing them onto the log pile
                    if (GameController.Instance.LogsRemaining == 0 || GameController.Instance.CurrentLogCount == 0)
                    {
                        Debug.Log("No logs");
                        return;
                    }
                    if (GameController.Instance.CurrentLogCount == 2)
                    {// Play  character animation sequence
                        weaponHandlerAnim.SetTrigger("GunUp");
                        LogHandlerAnim.SetTrigger("LogsDown");
                        if (GameController.Instance.LogsRemaining == 1)
                            GameController.Instance.CurrentLogCount = 1; // Prevents out of range error 
                    }
                    // Re enable the weapon
                    cur.enabled = true;// take away from the logs remaining counter
                    GameController.Instance.LogsRemaining -= GameController.Instance.CurrentLogCount;
                    GameController.Instance.PlaceLog(GameController.Instance.CurrentLogCount);
                    GameController.Instance.CurrentLogCount = 0;// increment the placed logs, reset held logs, and play sound
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Objective_Complete);
                }
                else if (hit.collider.CompareTag("AmmoPickup"))
                {
                    int r = Random.Range(0, WeaponManager.Instance.transform.childCount);
                    WeaponManager.Instance.transform.GetChild(r).GetComponent<GunController>().ClipAmount++;
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Extra);
                    Destroy(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("AssaultRiflePickup"))
                {// If the AR has already been picked up, grab ammo instead 
                    if (WeaponManager.Instance.UnlockedWeps[1] == true)
                    {
                        WeaponManager.Instance.transform.GetChild(1).GetComponent<GunController>().ClipAmount++;
                    }
                    WeaponManager.Instance.UnlockedWeps[1] = true;
                    WeaponManager.Instance.UpdateUnlockedUI(WeaponManager.Instance.GunPanelUI[1]);
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Gun);
                    Destroy(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("ShotgunPickup"))
                { // If the shotgun has already been picked up, grab ammo instead 
                    Debug.Log("S");
                    if (WeaponManager.Instance.UnlockedWeps[2] == true)
                    {
                        WeaponManager.Instance.transform.GetChild(2).GetComponent<GunController>().ClipAmount++;
                    }
                    WeaponManager.Instance.UnlockedWeps[2] = true;
                    WeaponManager.Instance.UpdateUnlockedUI(WeaponManager.Instance.GunPanelUI[2]);
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Gun);
                    Destroy(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("PistolPickup"))
                {
                    WeaponManager.Instance.transform.GetChild(0).GetComponent<GunController>().ClipAmount++;
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Extra);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
    private void MouseLook()
    {
        rotX = Input.GetAxis("Mouse X");
        rotY = Input.GetAxis("Mouse Y");
        // Debug.Log(rotY);
        // Have the camera rotate up and down, instead of the character
        camRot *= Quaternion.Euler(new Vector3(-rotY, 0, 0) * mouseSensitivity);
        camRot = ClampRotationAroundXAxis(camRot);
        //camRot.x = Mathf.Clamp(camRot.x, yMinRot, yMaxRot);
        // Have the char rotate left and right instead of up and down 
        charRot = Quaternion.Euler(new Vector3(0, rotX, 0) * mouseSensitivity);
        cam.transform.localRotation = camRot;
        transform.localRotation *= (charRot);
    }
    // This function was created by unity from the FirstPersonCharacters - Scripts - MouseLook.cs 
    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, yMinRot, yMaxRot);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool Grounded()
    {
        return false;
    }


    public static void ToggleCursour(bool torF)
    {
        // If not pressing left click, unlock cursour 
        if (torF == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;// Exit the loop
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateCurrentWeapon(GunController curr)
    {
        Cur = curr;
    }
}
