using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform targ;

    public float DampSpeed = 5.0f;

    void LateUpdate()
    {
        transform.position = Vector3.Slerp(transform.position,new Vector3(targ.position.x,transform.position.y,targ.position.z),Time.time * DampSpeed);
        Quaternion rot = 
        transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(90, targ.rotation.eulerAngles.y, 0),Time.time * DampSpeed);
    }
}
