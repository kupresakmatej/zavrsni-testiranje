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
        yield return new WaitForSeconds(2f);

        List<Vector3> walkableTilePositions = WalkableTiles.instance.GetWalkableTilePositions();

        if (walkableTilePositions.Count > 0)
        {
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            exitPosition = Vector3.zero;
            float minDistance = float.MaxValue;

            foreach (Vector3 position in walkableTilePositions)
            {
                float distance = Vector3.Distance(position, playerPosition);
                if (distance >= minDistanceFromPlayer && distance < minDistance)
                {
                    exitPosition = position;
                    minDistance = distance;
                }
            }

            if (exitPosition != Vector3.zero)
            {
                exitPosition.y += gameObject.transform.localScale.y / 2;

                getExitPosition = exitPosition;

                Instantiate(exitPrefab, exitPosition, Quaternion.identity);
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