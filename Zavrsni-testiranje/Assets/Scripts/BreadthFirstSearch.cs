using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour
{
    private List<Vector3> walkableTilePositions; // List of walkable tile positions
    private Vector3 startPosition; // Start position for pathfinding
    private Vector3 endPosition; // End position for pathfinding

    private Dictionary<Vector3, Vector3> cameFrom; // Map of positions to their previous positions in the path
    private Queue<Vector3> frontier; // Queue of positions to explore
    private HashSet<Vector3> visited; // Set of visited positions

    public List<Vector3> path = new List<Vector3>();

    public static BreadthFirstSearch instance;

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
            // loop through the path and log each position
            //foreach (Vector3 pos in path)
            //{
            //    Debug.Log($"Position: {pos}");
            //}

            UnityEngine.Debug.Log("BFS number of nodes in the path is: " + path.Count);
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
        cameFrom = new Dictionary<Vector3, Vector3>();
        frontier = new Queue<Vector3>();
        visited = new HashSet<Vector3>();

        frontier.Enqueue(start);
        visited.Add(start);

        while (frontier.Count > 0)
        {
            Vector3 current = frontier.Dequeue();

            if (current == end)
            {
                return GeneratePath(start, end);
            }

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null;
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
