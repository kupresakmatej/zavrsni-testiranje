using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    public Vector3 spawnPosition;

    public WalkableTiles walkableTiles;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(WaitForSeconds());
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(0.1f);

        // Get the list of walkable tile positions from the WalkableTiles script
        List<Vector3> walkablePositions = WalkableTiles.instance.GetWalkableTilePositions();

        // If there are no walkable positions, exit the function
        if (walkablePositions.Count == 0)
        {
            Debug.LogError("No walkable tiles found!");
        }

        // Choose a random walkable position to spawn the player on
        spawnPosition = walkablePositions[Random.Range(0, walkablePositions.Count)];

        spawnPosition.y += gameObject.transform.localScale.y / 2;

        // Instantiate the player prefab at the chosen spawn position
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
}