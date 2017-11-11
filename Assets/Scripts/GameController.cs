using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Text DeathTExt;
    public GameObject[] logUI;
    private int currentLogCount;// Logs being held on player
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
    private int logsRemaining = 20; // Logs required to finish the building 
    public int LogsRemaining
    {
        get { return logsRemaining; }
        set
        {
            logsRemaining = value;
            ui.StickRemaining.text = value.ToString();
            if(value %2 == 0)
            UpdateStickUI();
        }
    }
    public int LogsPlaced;// Logs placed on the pile 
    public Material[] LogMats = new Material[2];
    public Renderer[] LogChilds = new Renderer[20];
    [Header("Extra")]
    public UIManager ui;
    [Header("Timers")]
    public float GameTime;
    public delegate void SecondEvent();
    public event SecondEvent OnSecondChange;
    private int score;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            ui.Score.text = "Score: " + Score.ToString();
        }
    }

    public GameObject[] Lambo;

    private void Awake()
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
    private int totalSeconds;
    private void Update()
    {
        if (Time.time > GameTime + 1)
        {
            GameTime = Time.time;
            totalSeconds++;
            if (totalSeconds % 5 == 0) EnemyPooling.Instance.BaseZombiesPerMinute++;
            if (OnSecondChange != null)
            {
                OnSecondChange();
            }
        }
    }
    private void UpdateStickAnimations()
    {
        CharController.Instance.Cur.enabled = false;
        CharController.Instance.weaponHandlerAnim.SetTrigger("GunDown");// Put the players gun down, and enable logs animator
        CharController.Instance.LogHandlerAnim.SetTrigger("LogsUp");
    }

    private void UpdateStickUI()
    {
        logUI[LogsPlaced / 2].SetActive(true);
    }
    public void PlaceLog(int amount)
    {
        Debug.Log("Entered");
        for (int i = LogsPlaced; i < LogsPlaced + amount; i++)
        {
            LogChilds[i].material = LogMats[1];
            LogChilds[i].gameObject.GetComponent<BoxCollider>().isTrigger = false;

        }
        LogsPlaced += amount;
    }
    public void WinGame()
    {
        Lambo[0].GetComponent<Animator>().enabled = true;
        Lambo[1].SetActive(true);
        PlayerController.Instance.gameObject.SetActive(false);
    }

    public void LoseGame()
    {
        DeathTExt.enabled = true;
        Invoke("Restart3s",3);
        CharController.ToggleCursour(false);

    }

    private void Restart3s()
    {
        Destroy(PlayerController.Instance);
        Destroy(CharController.Instance);
        Destroy(Instance);
        SceneManager.LoadScene("MainMenuu");
    }
}

