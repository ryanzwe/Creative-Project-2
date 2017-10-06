using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class GameController : MonoBehaviour
{
    [Header("Gun Particles")]
    public int ImpactPSPool = 100;
    public GameObject HitPS;
    public GameObject[] HitPSPooled;
    private static GameController instance;
    public static GameController Instance
    {
        get { return instance; }
    }
    [Header("Logs")]
    public GameObject Log;
    public int CurrentLogCount = 0;
    public UIManager ui;
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
    }

    private void Update()
    {

    }
}

