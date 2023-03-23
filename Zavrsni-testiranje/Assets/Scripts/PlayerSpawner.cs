using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public WalkableTiles walkableTiles;
    public GameObject playerPrefab;

    void Start()
    {
        // Get the list of walkable tile positions from the WalkableTiles script
        List<Vector3> walkablePositions = walkableTiles.GetWalkableTiles();

        // If there are no walkable positions, exit the function
        if (walkablePositions.Count == 0)
        {
            Debug.LogError("No walkable tiles found!");
            return;
        }

        // Choose a random walkable position to spawn the player on
        Vector3 spawnPosition = walkablePositions[Random.Range(0, walkablePositions.Count)];

        spawnPosition.y += gameObject.transform.localScale.y / 2;

        // Instantiate the player prefab at the chosen spawn position
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
}