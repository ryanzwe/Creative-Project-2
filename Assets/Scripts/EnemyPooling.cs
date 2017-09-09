﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    public GameObject Zombie;
    public float BaseZombiesPerMinute = 45;
    public int EnemyAmount;
    public int ZombiesCreated = 0;

    private GameObject[] enemies;

    private Vector3[] SpawnPositions;

    private static EnemyPooling instance;

    public static EnemyPooling Instance
    {
        get { return instance; }
    }
    
    // Use this for initialization
    void Start()
    {
        instance = this;
        Transform spawnG = GameObject.Find("EnemySpawns").transform;
        SpawnPositions = new Vector3[spawnG.childCount];
        for (int i = 0; i < spawnG.childCount; i++)
        {
            SpawnPositions[i] = spawnG.GetChild(i).transform.position;
        }
        enemies = new GameObject[EnemyAmount];
        for (int i = 0; i < EnemyAmount; i++)
        {
            GameObject gb = Instantiate(Zombie);
            gb.SetActive(false);
            gb.transform.parent = this.transform;
            enemies[i] = gb;
            Create(SpawnPositions[i], Quaternion.identity);
        }
    }

    public GameObject Create(Vector3 pos, Quaternion rot)
    { // remove +1 if error                        To prevent the array from going out of length, will go back to the beginning once its greater than enemies.length
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].activeSelf)
            {
                ZombiesCreated++;
                enemies[i].SetActive(true);
                enemies[i].transform.position = pos;
                enemies[i].transform.rotation = rot;
                return enemies[i];
            }
        }
        return null; // Can't create the enemies;
    }

    public void Deactivate(GameObject gb)
    {
      gb.SetActive(false);
    }


    private float NextSpawn;
    void Update()
    {
        if (Time.time > NextSpawn)
        {
            NextSpawn = Time.time + (60 / BaseZombiesPerMinute);
            Create(SpawnPositions[ZombiesCreated % SpawnPositions.Length], Quaternion.identity);
        }
    }
}
