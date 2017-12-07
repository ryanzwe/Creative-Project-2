using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomisedEntitySpawner : MonoBehaviour
{
    [Range(1, 100)]
    public int MaxSpawns;

    public float SpawnsPerSecond = 1;
    public List<GameObject> CurrentEnties;
    public GameObject[] Entities;
    public GameObject LevelTerrain;
    private Collider levelRenderer;
    private Bounds levelBounds;
    private Vector3 levelSizing;
    private GameObject EntityHolder;

    private static RandomisedEntitySpawner instance;
    public static RandomisedEntitySpawner Instance => instance;
    private void Start()
    {
        instance = this;
        EntityHolder = new GameObject("Entity Holder");
        CurrentEnties = new List<GameObject>(MaxSpawns);
        StartCoroutine(EntitySpawning());
        SpawnsPerSecond /= 60;
        levelRenderer = LevelTerrain.GetComponent<Collider>();
        levelBounds = levelRenderer.bounds;
        
        //int a = 1 << 5 | 1 << 4;

        //int b = 187;// 10111011
        //int c = 128;// 10000000 
        //int d = 1 << 7; // same as c
        //int e = 255 >> 7; // = 1 because 100000 turns t00o 00000001 as it's shifted 7
     //   Debug.Log(e);
      //  Debug.Log(c==d);// since the bit in 7 is being pushed to 8, it will be true because 100000 is 128
      //  Debug.Log(a); //48 because 00110000
      //  Debug.Log(b&c); // 128 because B contains the byte C does 

    }

    private IEnumerator EntitySpawning()
    {
        while (true)
        {
            if (CurrentEnties.Count < MaxSpawns)
            {
                //Debug.Log("Inside");
                // Getting a random GameObject from the entities array
                int rand = Random.Range(0, Entities.Length);
                levelSizing = new Vector3
                (// Divide by two as it spawns from the middle, so 0 + levelbounds.size would be twice the size itshould be
                    Random.Range(-levelBounds.size.x /2 , levelBounds.size.x /2),
                    levelBounds.size.y + 15f,
                    Random.Range(-levelBounds.size.z /2, levelBounds.size.z /2)
                );
                GameObject ent = Entities[rand];
                // Setting a random point for the raycast dependant on the levels size
                Ray ray = new Ray(levelSizing, Vector3.down);
                RaycastHit hit;
                Debug.DrawRay(levelSizing, Vector3.down * 50000, Color.green, 15);
                int mask = 1 << 10;
                mask = ~mask;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        GameObject gb  = Instantiate(ent, hit.point, Quaternion.Euler(hit.normal.x, hit.normal.y + 1.5f, hit.normal.z));
                        CurrentEnties.Add(gb);
                        gb.transform.parent = EntityHolder.transform;
                    }
                }
            }
            yield return null; // Wait until next frame before next spawn
        }
    }

}
