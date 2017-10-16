using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get { return instance; }
    }
    public float health = 100f;
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            //GameController.Instance.ui.Health.value = health;
        }
    }
    public float TimeToHealToFullHealth = 60f;
    private float hps;


    private void OnEnable()
    {
        GameController.Instance.OnSecondChange += HealthOverTime;
    }
    private void OnDisable()
    {
        GameController.Instance.OnSecondChange -= HealthOverTime;
    }
    private void Start()
    {
        instance = this;
        hps = (health / TimeToHealToFullHealth); 
    }

    private void Update()
    {// Can't call this from a non monobehaviour ):, or through the property as it's still beign modified through the non monobehaviour
        if (GameController.Instance.ui.Health.value != health)
        {
            GameController.Instance.ui.Health.value = health;
        }
    }
    private void HealthOverTime()
    {
        if (health < 100)
        {
            health += hps;
            if (health > 100) health = 100;
        }
    }
}
