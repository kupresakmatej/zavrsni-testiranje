using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour
{
    private Dictionary<Vector3, List<Vector3>> graph = new Dictionary<Vector3, List<Vector3>>();

    private void Start()
    {
        StartCoroutine(WaitForSeconds());
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(5f);

        List<Vector3> walkableTilePostitions = WalkableTiles.instance.GetWalkableTilePositions();

        BuildGraph(walkableTilePostitions);

        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 exitPosition = GameObject.FindGameObjectWithTag("Exit").transform.position;

        List<Vector3> path = BFSearch(playerPosition, exitPosition);

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log("Path nodes: " + path[i]);
        }
    }

    private void BuildGraph(List<Vector3> walkableTilePositions)
    {
        foreach (Vector3 tilePos in walkableTilePositions)
        {
            List<Vector3> adjPositions = new List<Vector3>();

            // Check up
            Vector3 upPos = tilePos + Vector3.up;
            if (walkableTilePositions.Contains(upPos))
            {
                adjPositions.Add(upPos);
            }

            // Check down
            Vector3 downPos = tilePos + Vector3.down;
            if (walkableTilePositions.Contains(downPos))
            {
                adjPositions.Add(downPos);
            }

            // Check left
            Vector3 leftPos = tilePos + Vector3.left;
            if (walkableTilePositions.Contains(leftPos))
            {
                adjPositions.Add(leftPos);
            }

            // Check right
            Vector3 rightPos = tilePos + Vector3.right;
            if (walkableTilePositions.Contains(rightPos))
            {
                adjPositions.Add(rightPos);
            }

            // Add to graph
            graph.Add(tilePos, adjPositions);
        }
    }

    private List<Vector3> BFSearch(Vector3 start, Vector3 end)
    {
        // Initialize queue and visited set
        Queue<Vector3> queue = new Queue<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        // Add start node to queue and visited set
        queue.Enqueue(start);
        visited.Add(start);

        // Initialize parent dictionary
        Dictionary<Vector3, Vector3> parent = new Dictionary<Vector3, Vector3>();

        // Perform BFS
        while (queue.Count > 0)
        {
            // Get next node from queue
            Vector3 curr = queue.Dequeue();

            // Check if current node is the end node
            if (curr == end)
            {
                // Reconstruct path
                List<Vector3> path = new List<Vector3>();
                path.Add(end);

                while (parent.ContainsKey(path[0]))
                {
                    path.Insert(0, parent[path[0]]);
                }

                return path;
            }

            // Add neighbors to queue
            foreach (Vector3 neighbor in graph[curr])
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parent.Add(neighbor, curr);
                }
            }
        }

        // If end node was not found, return null
        return null;
    }
}
