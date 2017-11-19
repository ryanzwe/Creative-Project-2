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
            if (logsRemaining == 0)
                WinGame();
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
    public Animator PauseMenu;
    private bool paused;
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
            EndGame(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                PanelHandle(false);
                return;
            }
            PanelHandle(true);
        }
    }
    public void PanelHandle(bool TorF)
    {
        if(TorF)
        {
            SoundHandler.Instance.InitSliders();
            PauseMenu.SetTrigger("FlyIn");
            EndGame(false, false);
            paused = !paused;
        } else
        {
            SoundHandler.Instance.Running = false;
            PauseMenu.SetTrigger("FlyOut");
            paused = !paused;
            EndGame(false, true);
        }
    }

    #region StickAnimsPlacing
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
#endregion
    #region GameStates
    public void WinGame()
    {
        Lambo[0].GetComponent<Animator>().enabled = true;
        Lambo[1].SetActive(true);
        PlayerController.Instance.gameObject.SetActive(false);
        DisableInstances();
        EndGame(false);
    }

    public void EndGame(bool ShowDeathScreen,bool EnableAI = false)
    {
        if (ShowDeathScreen) SetupDeathCanvas();
        // Preventing movement and enabling cursour, also disable enemies from moving making the game appear paused 
        CharController.ToggleCursour(EnableAI);
        DisableInstances(EnableAI);
        for (int i = 0; i < EnemyPooling.Instance.enemies.Length; i++)
        {
            EnemyPooling.Instance.enemies[i].GetComponent<EnemyAI>().enabled = EnableAI;
            EnemyPooling.Instance.enemies[i].GetComponent<Animator>().enabled = EnableAI;
            EnemyPooling.Instance.enemies[i].GetComponent<NavMeshAgent>().enabled = EnableAI;
        }
    }
    public void SetupDeathCanvas()
    {
        // Grab the highscore, and see if the current score is higher than it, to set it 
        int _highscore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > _highscore) PlayerPrefs.SetInt("HighScore", score);
        // Setting the panel active that holds the texts
        DeathCanvas.SetActive(true);
        // Creating and setting up the way to interpolate the numbers
        AnimateNumber scoreAnim = gameObject.AddComponent<AnimateNumber>();
        AnimateNumber HighScoreAnim = gameObject.AddComponent<AnimateNumber>();
        scoreAnim.Setup(ref ScoreText, 0, Score, 1.5f);
        HighScoreAnim.Setup(ref HighScoreText, 0, _highscore, 1.5f);
        WinOrLoseText.text = PlayerController.Instance.Health > 0 ? "You Win!" : "You Lose!";
    }

    private void DisableInstances(bool t = true)
    {
        PlayerController.Instance.enabled = t;
        CharController.Instance.Cur.enabled = t;
        CharController.Instance.enabled = t;
        WeaponManager.Instance.enabled = t;
        EnemyPooling.Instance.enabled = t;
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

