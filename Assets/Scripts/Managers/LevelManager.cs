using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // takes care of the number of animals to spawn

    #region Spawn Animals Data
    [SerializeField]
    GameObject lion, dog, cat, chicken;
    [SerializeField]
    Vector3 spawnAreaCenter, spawnAreaSize;
    int numberOfAnimalsToSpawn;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        spawnAnimals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void spawnAnimals() {
        if(lion != null) {

        }
        if(dog != null) {

        }
        if(cat != null) {

        }
        if(chicken != null) {
            numberOfAnimalsToSpawn = GameManager.s_instance.numberOfChickens;
              for(int i = 0; i < numberOfAnimalsToSpawn; i++) {
                  Vector3 randomPos = getRandomSpawnPos();
                  Instantiate(chicken, randomPos, Quaternion.identity);
              }
        }
    }

    Vector3 getRandomSpawnPos() {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x * 0.5f, spawnAreaCenter.x + spawnAreaSize.x * 0.5f);
        float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z * 0.5f, spawnAreaCenter.z + spawnAreaCenter.z * 0.5f);

        return new Vector3(randomX, 0.5f, randomZ);
    }
}
