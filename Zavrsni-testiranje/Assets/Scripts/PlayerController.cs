using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public ExitSpawner exitSpawner;
    public float pathUpdateInterval = 0.5f;

    private List<Vector3> path = new List<Vector3>();
    private int currentPathIndex = 0;
    private float lastPathUpdateTime = 0f;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(pathUpdateInterval);

            if (exitSpawner != null)
            {
                Vector3 exitPosition = exitSpawner.GetExitPosition();

                if (exitPosition != Vector3.zero)
                {
                    // Use Dijkstra's algorithm to find the shortest path from the player's current position to the exit.
                    path = Dijkstra.FindShortestPath(transform.position, exitPosition, exitSpawner.walkableTiles);

                    currentPathIndex = 0;
                }
            }
        }
    }

    private void Update()
    {
        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            // Move towards the next point in the path.
            Vector3 targetPosition = path[currentPathIndex];
            targetPosition.y = transform.position.y;

            transform.LookAt(targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // If the player has reached the current point in the path, move to the next point.
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
    }
}