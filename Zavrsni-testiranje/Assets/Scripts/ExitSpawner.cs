using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpawner : MonoBehaviour
{
    public static ExitSpawner instance;
    public Vector3 exitPosition;

    public WalkableTiles walkableTiles;
    public int minDistanceFromPlayer = 5;
    public GameObject exitPrefab;

    private Vector3 getExitPosition;

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

    private void Start()
    {
        StartCoroutine(SpawnExit());
    }

    public Vector3 GetExitPosition()
    {
        return getExitPosition;
    }

    private IEnumerator SpawnExit()
    {
        yield return new WaitForSeconds(4f);

        List<Vector3> walkableTilePositions = WalkableTiles.instance.walkableTilePositions;

        if (walkableTilePositions.Count > 0)
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            // Define the minimum distance between the exit and the player
            float minDistanceFromPlayer = 150f;

            // Define the distance from the player to search for a suitable location to spawn the exit
            float searchDistance = 160f;

            List<Vector3> suitablePositions = new List<Vector3>();

            // Find all suitable positions within the search distance
            foreach (Vector3 position in walkableTilePositions)
            {
                float distance = Vector3.Distance(position, playerPosition);
                if (distance >= minDistanceFromPlayer && distance < searchDistance)
                {
                    suitablePositions.Add(position);
                }
            }

            if (suitablePositions.Count > 0)
            {
                // Select a random suitable position to spawn the exit
                int randomIndex = Random.Range(0, suitablePositions.Count);
                Vector3 exitPosition = suitablePositions[randomIndex];
                //exitPosition.y += gameObject.transform.localScale.y / 2;

                getExitPosition = exitPosition;

                Instantiate(exitPrefab, exitPosition, Quaternion.Euler(-90f, 0f, 0f));

                Debug.Log($"Generated exit position is {exitPosition}");
            }
            else
            {
                Debug.LogWarning("Unable to find a suitable location to spawn the exit.");
            }
        }
        else
        {
            Debug.LogWarning("No walkable tiles found.");
        }
    }
}