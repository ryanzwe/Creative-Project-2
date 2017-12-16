using UnityEngine;
using UnityEngine.Events;

public class Powerup : MonoBehaviour
{
    public PowerupEffect PowEffect;
    public string EffectnameTemp;

    public void Pickup()
    {
        // When picked up perform the Event, as well as calling Destruct once the ending event has been called
        PowEffect.Effect(); // Error saying this is null 
        PowEffect.EndEffect += delegate { Destruct(); };
        // Start a coroutine to call the ending effect after the desired time
        StartCoroutine(PowerupSystem.Instance.DelayedFunction(PowEffect.EndEffect, PowEffect.Duration));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Pickup();
            foreach (Transform trans in transform)
            {
                transform.GetComponent<BoxCollider>().enabled = false;
                trans.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    private void Destruct()
    {
        Destroy(gameObject);
    }

}
[SerializeField]
public class PowerupEffect
{
    public string Name;
    public float Duration;

    public UnityAction Effect;
    public UnityAction EndEffect;
    public PowerupEffect(string Name, float Duration, UnityAction Effect, UnityAction EndEffect)
    {
        this.Name = Name;
        this.Duration = Duration;
        this.Effect = Effect;
        this.EndEffect = EndEffect;
    }
}
