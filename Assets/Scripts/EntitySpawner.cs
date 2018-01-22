using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject EntityObject;
    private GameObject[] CurrentEntities;
    private Transform[] entityTransforms;
    private Renderer[] entityRenderers;
    private Collider[] entityColliders;
    private List<int> activeObjectAtPosition;
    public float Epm = 5;
    [Range(0,20)]
    public int MaxEntities;
    public Transform EntitySpawnPositionsHolder;
    private Vector3[] EntitySpawnPositions;

    IEnumerator Start()
    {
        // Set up the gameobjects and their spawn positions waiting a frame between each action to help framerate/initial load
        CurrentEntities = new GameObject[MaxEntities];
        EntitySpawnPositions = new Vector3[EntitySpawnPositionsHolder.childCount];
        entityTransforms = new Transform[EntitySpawnPositionsHolder.childCount];
        entityColliders = new Collider[EntitySpawnPositionsHolder.childCount];
        entityRenderers = new Renderer[EntitySpawnPositionsHolder.childCount];
        activeObjectAtPosition = new List<int>();
        for (int i = 0; i < EntitySpawnPositionsHolder.childCount; i++)
        {
            EntitySpawnPositions[i] = EntitySpawnPositionsHolder.GetChild(i).GetComponent<Transform>().position;
            yield return null;
        }
        for (int i = 0; i < MaxEntities; i++)
        {
            CurrentEntities[i] = Instantiate(EntityObject);
            entityTransforms[i] = CurrentEntities[i].GetComponent<Transform>();
            entityRenderers[i] = CurrentEntities[i].GetComponent<Renderer>();
            entityColliders[i] = CurrentEntities[i].GetComponent<Collider>();
            HideEnt(i);
            yield return null;
        }

        yield return new WaitForSeconds(2);
        for (int i = 0; i < 20; i++)
        {
            ShowEnt(RandomIndex(0,MaxEntities));
        }

    }

    public int RandomIndex(int min,int max)
    {
        int i = 0;
        // If the random number generated has been used before, regenerate it.
        do
        {
            i = Random.Range(min, max);
        } while (activeObjectAtPosition.Contains(i));


        return i;
    }

    public void ShowEnt(int i, bool random = false )
    {
        if(random)
            i = Random.Range(0, EntitySpawnPositions.Length);
        CurrentEntities[i].transform.position = EntitySpawnPositions[i];
       // CurrentEntities[i].transform.rotation = EntitySpawnPositionsHolder.GetChild(rInd).rotation;
        //Create a new ray pointing down to rotate the entity to the floor and ignore layer 10 by inversing the bits
        Ray ray = new Ray(EntitySpawnPositionsHolder.GetChild(i).position,Vector3.down);
        RaycastHit hit;
        int mask = 1 << 10;
        mask = ~mask;
        if (Physics.Raycast(ray, out hit, 5,mask))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                CurrentEntities[i].transform.rotation = Quaternion.Euler(hit.normal.x,hit.normal.y,hit.normal.z);
                CurrentEntities[i].transform.position = hit.point;
            }
                
            //Debug.Log(Quaternion.Euler(hit.point.x,hit.point.y,hit.point.z));
        }
        activeObjectAtPosition.Add(i);
        entityRenderers[i].enabled = entityColliders[i].enabled = true;
    }
    
    public void HideEnt(int i )
    {
        entityRenderers[i].enabled = entityColliders[i].enabled = false;
        activeObjectAtPosition.Remove(i);
    }


}
