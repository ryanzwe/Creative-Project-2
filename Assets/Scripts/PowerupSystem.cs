using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

public class PowerupSystem : MonoBehaviour
{
    public GameObject PowerupPrefab;
    public float DoublePointsDuration = 30, FreezeEnemiesDuration = 10, InstakillDuration = 25, InfiniteAmmoDuration = 15;
    public List<GameObject> CurrentPowerups = new List<GameObject>();

    private static PowerupSystem instance;
    public static PowerupSystem Instance => instance;
    public Dictionary<string, Vector3> PowerupSpawnPositions = new Dictionary<String, Vector3>(8);
    public Dictionary<PowerUps, PowerupEffect> PowerupLibrary = new Dictionary<PowerUps, PowerupEffect>()
    {

        {PowerUps.DoublePoints,new PowerupEffect("DoublePoints",30,
            delegate{GameController.Instance.ScoreMultiplier = 2; },
            delegate{GameController.Instance.ScoreMultiplier = 1; })
        },
        {PowerUps.FreezeZombies,new PowerupEffect("FreezeZombies",10,
            delegate{GameController.ToggleAI(false); },
            delegate{GameController.ToggleAI(true); })
        },
        {PowerUps.InstaKill,new PowerupEffect("InstaKill",10,
            delegate{for (int i = 0; i < EnemyPooling.Instance.EnemyAmount; i++)
            {
              EnemyPooling.Instance.EnemyBrains[i].health = 1;
            } },
            delegate{for (int i = 0; i < EnemyPooling.Instance.EnemyAmount; i++)
            {
                EnemyPooling.Instance.EnemyBrains[i].health = EnemyPooling.Instance.EnemyBrains[i].MaxHealth ; }
            } )
        },
        {PowerUps.InfiniteAmmo,new PowerupEffect("InfiniteAmmo",10,
            delegate{for (int i = 0; i < WeaponManager.Instance.Guns.Length; i++)
            {
                    WeaponManager.Instance.Guns[i].CurrentClip = 999;
            }},
            delegate{for (int i = 0; i < WeaponManager.Instance.Guns.Length; i++)
            {
                    WeaponManager.Instance.Guns[i].CurrentClip = WeaponManager.Instance.Guns[i].ClipSize;
            }})
        }

    };

    private void Start()
    {
        instance = this;
      
        PowerupSpawnPositions.Add("N", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.center.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.max.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("NE", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.max.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.max.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("E", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.max.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.center.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("SE", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.max.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.min.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("S", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.center.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.min.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("SW", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.min.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.min.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("W", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.min.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.center.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);
        PowerupSpawnPositions.Add("NW", new Vector3(
            RandomisedEntitySpawner.Instance.LevelBounds.min.x,
            RandomisedEntitySpawner.Instance.LevelBounds.min.y + 2.5f,
            RandomisedEntitySpawner.Instance.LevelBounds.max.z) * RandomisedEntitySpawner.Instance.LevelDampingRange);

        for (int i = 0; i < PowerupSpawnPositions.Count; i++)
        {
            Vector3 position = PowerupSpawnPositions.Values.ElementAt(i);
            position.y += 25;
            Ray ray = new Ray(position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5000))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    position = hit.point;
                    //position.y += 0.5f;
                    Quaternion rotation = Quaternion.Euler(hit.normal.x, hit.normal.y, hit.normal.z);
                    SpawnPowerup((PowerUps)UnityEngine.Random.Range(0, 3), position, rotation);
                }
            }
        }
    }

    public enum PowerUps
    {
        DoublePoints, FreezeZombies, InstaKill, InfiniteAmmo
    };
    /*
     * Remove hard coded values, save their previous values in variables here
     * */


    public void AddPowerup(GameObject P)
    {
        CurrentPowerups.Add(P);
    }

    public void RemovePowerup(GameObject P)
    {
        CurrentPowerups.Remove(P);
    }
    public void SpawnPowerup(PowerUps powerUp, Vector3 position, Quaternion rotation)
    {
        GameObject powerup = Instantiate(PowerupPrefab, position, rotation);
        powerup.name = powerUp.ToString();

        Powerup pow = powerup.AddComponent<Powerup>();

        pow.PowEffect = PowerupLibrary[powerUp];

        // Debug.Log(powerUp);

        AddPowerup(powerup);
    }

    public IEnumerator DelayedFunction(UnityAction e, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        e?.Invoke();
    }

}
// PSlit this powerup class into its own script 



