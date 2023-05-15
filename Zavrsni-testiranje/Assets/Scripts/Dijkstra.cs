using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    private List<Vector3> walkableTilePositions; // List of walkable tile positions
    private Vector3 startPosition; // Start position for pathfinding
    private Vector3 endPosition; // End position for pathfinding

    private Dictionary<Vector3, float> distances; // Map of positions to their tentative distances
    private Dictionary<Vector3, Vector3> cameFrom; // Map of positions to their previous positions in the path
    private HashSet<Vector3> visited; // Set of visited positions

    public List<Vector3> path = new List<Vector3>();

    public static Dijkstra instance;

    private void Awake()
    {
        path.Clear();

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
        StartCoroutine(GetPathList());
    }

    private IEnumerator GetPathList()
    {
        Stopwatch stopwatch = new Stopwatch();

        // Start the stopwatch
        stopwatch.Start();

        yield return new WaitForSeconds(6f);

        walkableTilePositions = WalkableTiles.instance.walkableTilePositions;

        startPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        endPosition = GameObject.FindGameObjectWithTag("Exit").transform.position;

        List<Vector3> path = FindPath(startPosition, endPosition);
        if (path != null)
        {
            UnityEngine.Debug.Log($"Dijkstra path found with length {path.Count}");
        }
        else
        {
            UnityEngine.Debug.Log("No path found!");
        }

        stopwatch.Stop();

        // Get the elapsed time in milliseconds
        long elapsedMillis = stopwatch.ElapsedMilliseconds;

        var seconds = (elapsedMillis / 1000) % 60;
        var minutes = (elapsedMillis / 1000) / 60;

        UnityEngine.Debug.Log("Time taken: " + minutes + ":" + seconds);
    }

    private List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        distances = new Dictionary<Vector3, float>();
        cameFrom = new Dictionary<Vector3, Vector3>();
        visited = new HashSet<Vector3>();

        foreach (Vector3 tilePos in walkableTilePositions)
        {
            distances[tilePos] = Mathf.Infinity;
        }

        distances[start] = 0;
        cameFrom[start] = start;

        while (visited.Count < walkableTilePositions.Count)
        {
            Vector3 current = GetNextVertex();
            visited.Add(current);

            if (current == end)
            {
                return GeneratePath(start, end);
            }

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                float distanceToNeighbor = Vector3.Distance(current, neighbor);
                float tentativeDistance = distances[current] + distanceToNeighbor;

                if (tentativeDistance < distances[neighbor])
                {
                    distances[neighbor] = tentativeDistance;
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null;
    }

    private Vector3 GetNextVertex()
    {
        Vector3 nextVertex = Vector3.zero;
        float shortestDistance = Mathf.Infinity;

        foreach (Vector3 tilePos in walkableTilePositions)
        {
            if (!visited.Contains(tilePos) && distances[tilePos] < shortestDistance)
            {
                nextVertex = tilePos;
                shortestDistance = distances[tilePos];
            }
        }

        return nextVertex;
    }

    private List<Vector3> GetNeighbors(Vector3 current)
    {
        List<Vector3> neighbors = new List<Vector3>();

        foreach (Vector3 tilePos in walkableTilePositions)
        {
            if (Vector3.Distance(current, tilePos) <= 1.1f) // check if tile is within range
            {
                neighbors.Add(tilePos);
            }
        }

        return neighbors;
    }

    private List<Vector3> GeneratePath(Vector3 start, Vector3 end)
    {
        Vector3 current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }
}