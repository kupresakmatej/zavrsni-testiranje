using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private List<Vector3> walkableTilePositions; // List of walkable tile positions
    private Vector3 startPosition; // Start position for pathfinding
    private Vector3 endPosition; // End position for pathfinding

    private Dictionary<Vector3, float> gScores; // Map of positions to their G scores (tentative distances from the start node)
    private Dictionary<Vector3, float> fScores; // Map of positions to their F scores (estimated total distances from start to goal through that position)
    private Dictionary<Vector3, Vector3> cameFrom; // Map of positions to their previous positions in the path
    private HashSet<Vector3> closedSet; // Set of positions already evaluated
    private HashSet<Vector3> openSet; // Set of tentative positions to be evaluated, initially containing the start position

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
            UnityEngine.Debug.Log($"Path found with length {path.Count}");
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
        gScores = new Dictionary<Vector3, float>();
        fScores = new Dictionary<Vector3, float>();
        cameFrom = new Dictionary<Vector3, Vector3>();
        closedSet = new HashSet<Vector3>();
        openSet = new HashSet<Vector3>();

        foreach (Vector3 tilePos in walkableTilePositions)
        {
            gScores[tilePos] = Mathf.Infinity;
            fScores[tilePos] = Mathf.Infinity;
        }

        gScores[start] = 0;
        fScores[start] = HeuristicCostEstimate(start, end);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Vector3 current = GetLowestFScore(openSet);

            if (current == end)
            {
                return GeneratePath(start, end);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScores[current] + Vector3.Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScores[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScores[neighbor] = tentativeGScore;
                fScores[neighbor] = gScores[neighbor] + HeuristicCostEstimate(neighbor, end);
            }
        }

        return null;
    }

    private Vector3 GetLowestFScore(HashSet<Vector3> positions)
    {
        Vector3 lowestPosition = Vector3.zero;
        float lowestScore = Mathf.Infinity;

        foreach (Vector3 position in positions)
        {
            float score = fScores[position];

            if (score < lowestScore)
            {
                lowestScore = score;
                lowestPosition = position;
            }
        }

        return lowestPosition;
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        List<Vector3> neighbors = new List<Vector3>();

        foreach (Vector3 tilePos in walkableTilePositions)
        {
            if (Vector3.Distance(position, tilePos) == 1f)
            {
                neighbors.Add(tilePos);
            }
        }

        return neighbors;
    }

    private List<Vector3> GeneratePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();

        return path;
    }

    private float HeuristicCostEstimate(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end);
    }
}