using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // takes care of the number of animals to spawn

    #region Spawn Animals Data
    [SerializeField]
    GameObject lion, dog, cat, chicken;
    [SerializeField]
    LevelGenerator levelGenerator;
    int numberOfAnimalsToSpawn;

    int gridWidth, gridDepth;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gridWidth = levelGenerator.gridWidth;
        gridDepth = levelGenerator.gridDepth;
        spawnAnimals();
    }


    void spawnAnimals() {
        if(lion != null) {
            numberOfAnimalsToSpawn = GameManager.s_instance.numberOfLions;
            for (int i = 0; i < numberOfAnimalsToSpawn; i++) {
                //Vector3 randomPos = getRandomSpawnPos();
                Vector3 spawnPosition = GetValidSpawnPosition();
                Instantiate(lion, spawnPosition, Quaternion.identity);
            }
        }
        if(dog != null) {
            numberOfAnimalsToSpawn = GameManager.s_instance.numberOfDogs;
            for (int i = 0; i < numberOfAnimalsToSpawn; i++) {
                Vector3 spawnPosition = GetValidSpawnPosition();
                Instantiate(dog, spawnPosition, Quaternion.identity);
            }
        }
        if(cat != null) {
            numberOfAnimalsToSpawn = GameManager.s_instance.numberOfCats;
            for(int i = 0; i < numberOfAnimalsToSpawn; i++) {
                Vector3 spawnPosition = GetValidSpawnPosition();
                Instantiate(cat, spawnPosition, Quaternion.identity);
            }
        }
        if(chicken != null) {
            numberOfAnimalsToSpawn = GameManager.s_instance.numberOfChickens;
            for(int i = 0; i < numberOfAnimalsToSpawn; i++) {
                Vector3 spawnPosition = GetValidSpawnPosition();
                Instantiate(chicken, spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetValidSpawnPosition() {
        Vector3 spawnPosition = Vector3.zero;

        do {
            // Get a random spawn position
            spawnPosition = GetRandomSpawnPosition();
        }
        // Check if the spawn position is on top of a water cubeCell
        while (IsPositionOnWaterCube(spawnPosition));

        spawnPosition = new Vector3(spawnPosition.x, 0.5f, spawnPosition.z);

        return spawnPosition;
    }

    bool IsPositionOnWaterCube(Vector3 position) {
        // Convert the world position to grid coordinates
        int i = Mathf.RoundToInt(position.x + gridWidth * 0.5f);
        int j = Mathf.RoundToInt(position.z + gridDepth * 0.5f);

        // Check if the grid position corresponds to a water cubeCell
        return levelGenerator.getCubeMap()[i, j] == CellType.Water;
    }

    Vector3 GetRandomSpawnPosition() {
        // Generate random grid coordinates within the grid boundaries
        int i = Random.Range(0, gridWidth);
        int j = Random.Range(0, gridDepth);

        // Calculate world position based on grid coordinates
        Vector3 spawnPosition = new Vector3(i - gridWidth * 0.5f, 0, j - gridDepth * 0.5f);

        return spawnPosition;
    }
}
