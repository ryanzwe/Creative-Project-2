﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPooling : MonoBehaviour
{
    public GameObject[] Zombies;
    public EnemyAI[] EnemyBrains;
    public float BaseZombiesPerMinute = 45;
    public int EnemyAmount;
    public int ZombiesCreated = 0;

    public GameObject[] enemies;

    private Vector3[] SpawnPositions;

    private static EnemyPooling instance;

    public static EnemyPooling Instance => instance;

    // Use this for initialization
    void Start()
    {
        instance = this;

        Transform spawnG = GameObject.Find("EnemySpawns").transform;
        SpawnPositions = new Vector3[spawnG.childCount];
        enemies = new GameObject[EnemyAmount];
        EnemyBrains = new EnemyAI[EnemyAmount];
        for (int i = 0; i < spawnG.childCount; i++)
        {
            SpawnPositions[i] = spawnG.GetChild(i).transform.position;
        }
        for (int i = 0; i < EnemyAmount; i++)
        {
            // Grabbing a random zombie from the zombies array 
            GameObject gb = Instantiate(Zombies[Random.Range(0, Zombies.Length)]);
            // Adding extra scale onto the transform to add variation onto their original sizing
            float randHeightAdditional = Random.Range(0f, 0.15f);
            Transform zombT = gb.transform;
            gb.transform.localScale = new Vector3(zombT.localScale.x + randHeightAdditional, zombT.localScale.y + randHeightAdditional, zombT.localScale.z + randHeightAdditional);
            // setting them active and placing them under the parent, as well as adding them to the array then calling the create method on them
            gb.SetActive(false);
            gb.transform.parent = this.transform;
            enemies[i] = gb;
            EnemyBrains[i] = gb.GetComponent<EnemyAI>();
            gb.name = "Zombie " + i;
            Create(SpawnPositions[i], Quaternion.identity);
        }
        //GetComponent<EnemyAI>().enabled = true;
    }

    public GameObject Create(Vector3 pos, Quaternion rot)
    { // remove +1 if error                        To prevent the array from going out of length, will go back to the beginning once its greater than enemies.length
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].activeSelf)
            {
                ZombiesCreated++;
                enemies[i].GetComponent<NavMeshAgent>().Warp(pos);// If i do setposition then the agent gets confused and doesn't get set properly.
                enemies[i].GetComponent<NavMeshAgent>().enabled = true;
                enemies[i].SetActive(true);
               // enemies[i].transform.position = pos;
                enemies[i].transform.rotation = rot;
                EnemyBrains[i].health = EnemyBrains[i].MaxHealth;
                return enemies[i]; // Exit the loops once this has been spawned
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
