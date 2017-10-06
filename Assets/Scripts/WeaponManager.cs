using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int wepIndex;
    private Animator anim;
    private void Start()
    {
    EquipWeapon();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Switching();
    }
    void EquipWeapon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == wepIndex)
                transform.GetChild(i).gameObject.SetActive(true);
            else
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Switching()
    {
        int prevIndex = wepIndex;

        if (Input.GetAxis("Mouse ScrollWheel") >  0f)
        {     // If scrolling outside the array then loop back to the start              
            if (wepIndex >= transform.childCount - 1)
                wepIndex = 0;
            else // If not then incremednt to the next wep
                wepIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        { // If going lower than the array, then loop back to the end, -1 to prevent the array from going out of range
            if (wepIndex <= 0)
                wepIndex = transform.childCount - 1;
            else
                wepIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wepIndex = 0;
        }// bug: change transform.chilcount to if bought
        else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            wepIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            wepIndex = 2;
        }

        // If the wep index changes at the end of the frame, then reselect a wep
        if (wepIndex != prevIndex)
            EquipWeapon();
    }
    public void AnimationEventShootfalse()
    {
        anim.SetBool("Shooting", false);
    }
}
