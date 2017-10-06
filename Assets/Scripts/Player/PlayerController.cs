using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float health;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            GameController.Instance.ui.Health.value = health;
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
    }

}
