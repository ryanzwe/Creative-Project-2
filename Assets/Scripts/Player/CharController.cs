using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal.Input;

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
    private float mouseSensitivity = 5.0f;

    //Forces 
    private Rigidbody rb;
    private float moveSpeed = 2.0f;
    private float jumpSpeed = 2f;

    private bool sprinting = false;
    // Extra
    private bool cursLocked = false;
    private GameObject cam;
    public float PickupDistance = 5f;
    public Animator weaponHandlerAnim;

    private static CharController instance;
    public static CharController Instance
    {
        get { return instance; }
        private set {}
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        instance = this;
        cam = transform.GetChild(0).gameObject;
        camRot = cam.transform.localRotation;
        charRot = transform.localRotation;
        ToggleCursour(true);
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
    {
        if (Input.GetKey(KeyCode.LeftShift)) sprinting = true;
        else sprinting = false;
        float sprintModifier = sprinting ? 2 : 1;

        Vector3 prePos = transform.position;
        verticalMovement = Input.GetAxis("Vertical") * moveSpeed * sprintModifier * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * sprintModifier * Time.deltaTime;
        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement));
        if (transform.position != prePos && sprinting) weaponHandlerAnim.SetBool("Walking", true); else weaponHandlerAnim.SetBool("Walking", false);
        // bug: rename from walking to sprinting in animator, make so cant shoot while sprinting in guncontroller
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
                if (hit.collider.CompareTag("Log"))
                {// Make the log fly towards the player 
                    Vector3 dir = new Vector3(transform.position.x -hit.transform.position.x, transform.position.y -  hit.transform.position.y + 2f, transform.position.z - hit.transform.position.z);
                    Physics.IgnoreCollision(hit.collider,GetComponent<Collider>());// Disable collision so it doesn't push the player 
                    hit.collider.GetComponent<Rigidbody>().AddForce(dir*2f,ForceMode.Impulse);// Adding the force towards the player 
                    Destroy(hit.collider.gameObject,1f);// destroying the log after as econd 
                    GameController.Instance.CurrentLogCount++;// adding one to the playes log count, for game completion
                    SoundHandler.Instance.PlaySound(SoundHandler.Sounds.Pickup_Extra);// as this is an extra item, play this sound 
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
}
