using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyinEnemySpawnerTemp : MonoBehaviour
{

    public GameObject FlyingZombie;
    private GameObject FlyingZombieParent;
    public int AmountToSpawn;
    private int spawnedAmt;
    public void Start()
    {
        FlyingZombieParent = new GameObject("Flying  Zombies Holder");
        StartCoroutine(SpawnZombies());
    }
    IEnumerator SpawnZombies()
    {
        while (AmountToSpawn > spawnedAmt)
        {
            yield return new WaitForSeconds(1f);
            GameObject gb = Instantiate(FlyingZombie);
            gb.transform.parent = FlyingZombieParent.transform;
            spawnedAmt++;
        }
    }

}
