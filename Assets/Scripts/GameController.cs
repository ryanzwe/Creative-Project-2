using UnityEngine;
using UnityEngine.AI;
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
            if (value % 2 == 0)
                UpdateStickUI();
        }
    }
    public int LogsPlaced;// Logs placed on the pile 
    public Material[] LogMats = new Material[2];
    public Renderer[] LogChilds = new Renderer[20];
    [Header("Extra")]
    public UIManager ui;
    public GameObject DeathCanvas;
    public Text ScoreText;
    public Text HighScoreText;
    public Text WinOrLoseText;
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
        if(Input.GetKeyDown(KeyCode.F))
        {
            EndGame();
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
    #region GameStates
    public void WinGame()
    {
        Lambo[0].GetComponent<Animator>().enabled = true;
        Lambo[1].SetActive(true);
        PlayerController.Instance.gameObject.SetActive(false);
        DisableInstances();
    }

    public void EndGame()
    {
        // Setting the panel active that holds the texts
        DeathCanvas.SetActive(true);
        // Creating and setting up the way to interpolate the numbers
        AnimateNumber scoreAnim = gameObject.AddComponent<AnimateNumber>();
        AnimateNumber HighScoreAnim = gameObject.AddComponent<AnimateNumber>();
        scoreAnim.Setup(ref ScoreText, 0, Score, 1.5f);
        HighScoreAnim.Setup(ref ScoreText, 0, PlayerPrefs.GetInt("HighScore"), 1.5f);
        WinOrLoseText.text = PlayerController.Instance.Health > 0 ? "You Win!" : "You Lose!";
        // Preventing movement and enabling cursour 
        CharController.ToggleCursour(false);
        DisableInstances();
        for (int i = 0; i < EnemyPooling.Instance.enemies.Length; i++)
        {
            EnemyPooling.Instance.enemies[i].GetComponent<EnemyAI>().enabled = false;
            EnemyPooling.Instance.enemies[i].GetComponent<Animator>().enabled = false;
            EnemyPooling.Instance.enemies[i].GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    private void DisableInstances()
    {
        PlayerController.Instance.enabled = false;
        CharController.Instance.Cur.enabled = false;
        CharController.Instance.enabled = false;
        WeaponManager.Instance.enabled = false;
        EnemyPooling.Instance.enabled = false;
    }
    public void RestartGame()
    {
        Destroy(PlayerController.Instance);
        Destroy(CharController.Instance);
        Destroy(WeaponManager.Instance);
        Destroy(EnemyPooling.Instance);
        Destroy(Instance);
        SceneManager.LoadScene("MainMenuu");
    }
    #endregion
}

