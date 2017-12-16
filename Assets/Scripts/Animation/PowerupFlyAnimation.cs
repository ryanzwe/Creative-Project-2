using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupFlyAnimation : MonoBehaviour
{
    private Transform trans;
    // Use this for initialization
    void Start()
    {
        trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        trans.position = new Vector3(trans.position.x, trans.position.y + Mathf.Sin(Time.time) * 0.005f, trans.position.z);
        trans.Rotate(new Vector3(45, 45, 45) * Time.deltaTime);
    }
}
