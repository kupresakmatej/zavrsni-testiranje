using System.Collections.Generic;
using UnityEngine;

public static class Dijkstra
{
    public static List<Vector3> FindShortestPath(Vector3 start, Vector3 end, WalkableTiles walkableTiles)
    {
        Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>();
        Dictionary<Vector3, Vector3> previous = new Dictionary<Vector3, Vector3>();
        HashSet<Vector3> unvisited = new HashSet<Vector3>(walkableTiles.GetWalkableTiles());

        foreach (Vector3 position in walkableTiles.GetWalkableTiles())
        {
            distances[position] = float.MaxValue;
            previous[position] = Vector3.zero;
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            Vector3 current = Vector3.zero;
            float shortestDistance = float.MaxValue;

            foreach (Vector3 position in unvisited)
            {
                if (distances[position] < shortestDistance)
                {
                    shortestDistance = distances[position];
                    current = position;
                }
            }

            if (current == end)
            {
                break;
            }

            unvisited.Remove(current);

            foreach (Vector3 neighbor in GetNeighbors(current, walkableTiles))
            {
                float distance = Vector3.Distance(current, neighbor);

                if (distances[current] + distance < distances[neighbor])
                {
                    distances[neighbor] = distances[current] + distance;
                    previous[neighbor] = current;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        Vector3 currentPathNode = end;

        while (currentPathNode != start)
        {
            path.Add(currentPathNode);
            currentPathNode = previous[currentPathNode];
        }

        path.Reverse();

        return path;
    }

    private static List<Vector3> GetNeighbors(Vector3 position, WalkableTiles walkableTiles)
    {
        List<Vector3> neighbors = new List<Vector3>();
        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPosition = position + direction;

            if (walkableTiles.IsWalkable(neighborPosition))
            {
                neighbors.Add(neighborPosition);
            }
        }

        return neighbors;
    }
}