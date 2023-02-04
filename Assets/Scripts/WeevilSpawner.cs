using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeevilSpawner : MonoBehaviour
{
    // Spawn variables that we'll put in Unity for tweaking
    [Header("Weevil Spawn Data")]
    [SerializeField] RootGenerator rootGenerator;
    [SerializeField] GameObject weevilPrefab;
    [Range(0.5f, 5.0f)]
    [SerializeField] float minSpawnTime = .5f;
    [Range(0.5f, 5.0f)]
    [SerializeField] float maxSpawnTime = .5f;

    // constant values for spawning this critter
    private static float spawnRange = 6.0f;

    // public variables used by this script and others
    public int numToSpawn = 1;

    // private variables used by this script
    private float spawnTimer;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // set up the spawn timer
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            // reset the spawn timer
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

            // spawn the number of weevils that is currently set up
            for (int numWeevils = 0; numWeevils < numToSpawn; numWeevils++)
            {
                SpawnWeevil();
            }
        }
        
    } // end Update

    /// <summary>
    /// spwans a new weevil in the proper range
    /// </summary>
    private void SpawnWeevil()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRange, spawnRange), 
                                       weevilPrefab.transform.position.y,
                                       weevilPrefab.transform.position.z);

        GameObject spawnedWeevil = Instantiate(weevilPrefab, spawnPos, Quaternion.identity);
        spawnedWeevil.GetComponent<WeevilMovement>().rootGenerator = rootGenerator;
    }
}
