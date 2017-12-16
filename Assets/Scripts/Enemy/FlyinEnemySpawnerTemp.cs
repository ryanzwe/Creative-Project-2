using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyinEnemySpawnerTemp : MonoBehaviour
{

    public GameObject FlyingZombie;
    public GameObject PowerupPrefab;
    private MeshRenderer meshR;
    private BoxCollider col;
    public int ScoreValue = 150;

    public void Start()
    {
        meshR = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider>();
        GameController.Instance.OnMilestoneHit += SpawnFlying;
    }
    public void ToggleZombie(bool show )
    {
        if (meshR != null && col != null)
        {
            meshR.enabled = show;
            col.enabled = show;
            if(show == false)
            {
                // This means that the zombie has been killed
                // Spawn powerup
                GameController.Instance.Score += ScoreValue;
                ToggleZombie(false);
                Debug.Log("Killed Zombie");
            }
        }
    }
    private void SpawnFlying()
    {
        Instantiate(FlyingZombie);
        Debug.Log("Memes");
    }
}
