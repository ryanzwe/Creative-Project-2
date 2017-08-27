using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float moveSpeed = 5.0f;
    private float jumpSpeed = 2f;
    // Extra
    private bool cursLocked = false;
    private GameObject cam;

    private static CharController instance;

    public static CharController Instance
    {
        get { return instance ?? (GameObject.FindObjectOfType<CharController>()); }
    }

    private void Start()
    {
        cam = transform.GetChild(0).gameObject;
        camRot = cam.transform.localRotation;
        charRot = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        Movement();
        MouseLook();
    }

    private void Movement()
    {
        verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement));
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursour(false);
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
