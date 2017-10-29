using UnityEngine;


public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance
    {
        get { return instance; }
    }
    [Header("Gun Particles")]
    public int ImpactPSPool = 100;
    public GameObject HitPS;
    public GameObject[] HitPSPooled;
    [Header("Logs")]
    public GameObject Log;

    private int currentLogCount;
    private int logsRemaining = 20;
    public int LogsRemaining
    {
        get { return logsRemaining; }
        set
        {
            logsRemaining = value;
            ui.StickRemaining.text = value.ToString();
        }
    }

    public int CurrentLogCount // Update UI, Trigger 
    {
        get { return currentLogCount; }
        set
        {
            currentLogCount = value;
            ui.StickCount.text = currentLogCount.ToString();
            if (currentLogCount == 2)
            {
                UpdateStickAnimations();

            }
        }
    }

    

    [Header("Extra")]
    public UIManager ui;
    [Header("Timers")]
    public float GameTime;

    public delegate void SecondEvent();

    public event SecondEvent OnSecondChange;
    void Awake()
    {// Setting the instance and the parent to hold the pooled objects
        instance = this;
        GameObject ImpactPsHolder = new GameObject("ImpactPsHolder");
        ImpactPsHolder.transform.parent = transform;
        // Setting the length of the array
        HitPSPooled = new GameObject[ImpactPSPool];
        // pooling and filling 
        for (int i = 0; i < ImpactPSPool; i++)
        {
            GameObject ps = Instantiate(HitPS);
            ps.transform.parent = ImpactPsHolder.transform;
            HitPSPooled[i] = ps;
            ps.SetActive(false);
        }
        GameTime = 0;
    }

    private void Update()
    {
        if (Time.time > GameTime + 1)
        {
            GameTime = Time.time;
            if (OnSecondChange != null) OnSecondChange();
        }
    }
    private void UpdateStickAnimations()
    {
        CharController.Instance.Cur.enabled = false;
        CharController.Instance.weaponHandlerAnim.SetTrigger("GunDown");// Put the players gun down, and enable logs animator
        CharController.Instance.LogHandlerAnim.SetTrigger("LogsUp");
    }
}

