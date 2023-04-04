using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    private Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();

    private List<Vector3> walkableTilePositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(WaitForSpawns());
    }

    public List<Vector3> FindShortestPath(Vector3 start, Vector3 end)
    {
        Debug.Log("FindShortestPath called with start: " + start + " and end: " + end);

        if (!nodes.ContainsKey(start))
        {
            Debug.LogError("Start position not found in nodes dictionary: " + start);
            return null;
        }

        if (!nodes.ContainsKey(end))
        {
            Debug.LogError("End position not found in nodes dictionary: " + end);
            return null;
        }

        Dictionary<Vector3, float> distance = new Dictionary<Vector3, float>();
        Dictionary<Vector3, Vector3> previous = new Dictionary<Vector3, Vector3>();
        HashSet<Vector3> unvisited = new HashSet<Vector3>();

        foreach (Node node in nodes.Values)
        {
            distance[node.position] = float.MaxValue;
            previous[node.position] = Vector3.zero;
            unvisited.Add(node.position);
        }

        distance[start] = 0;

        while (unvisited.Count > 0)
        {
            Vector3 current = Vector3.zero;
            float shortestDistance = float.MaxValue;

            foreach (Vector3 position in unvisited)
            {
                if (distance[position] < shortestDistance)
                {
                    current = position;
                    shortestDistance = distance[position];
                }
            }

            unvisited.Remove(current);

            if (current == end)
            {
                break;
            }

            if (!nodes.ContainsKey(current))
            {
                Debug.LogError("Key not found in nodes dictionary: " + current);
                break;
            }

            foreach (Node neighbor in nodes[current].adjacentNodes)
            {
                float alternateDistance = distance[current] + Mathf.Abs(current.x - neighbor.position.x) + Mathf.Abs(current.y - neighbor.position.y);

                if (alternateDistance < distance[neighbor.position])
                {
                    distance[neighbor.position] = alternateDistance;
                    previous[neighbor.position] = current;

                    Debug.Log("Adding to previous dictionary: " + neighbor.position + " -> " + current);
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        Vector3 currentPos = end;

        while (currentPos != start)
        {
            path.Add(currentPos);

            if (!previous.ContainsKey(currentPos))
            {
                Debug.LogError("Key not found in previous dictionary: " + currentPos);
                break;
            }
            else
            {
                currentPos = previous[currentPos];
            }
        }

        path.Add(start);
        path.Reverse();

        Debug.Log("Shortest path found: " + string.Join(", ", path));

        return path;
    }

    private IEnumerator WaitForSpawns()
    {
        yield return new WaitForSeconds(5f);

        walkableTilePositions = WalkableTiles.instance.GetWalkableTilePositions();

        foreach (Vector3 position in walkableTilePositions)
        {
            Node node = new Node { position = position };
            nodes[position] = node;
            Debug.Log("Added node: " + node.position);
        }

        // Check for adjacent nodes for each node
        foreach (Node node in nodes.Values)
        {
            Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

            foreach (Vector3 direction in directions)
            {
                Vector3 adjacentPosition = node.position + direction;

                if (nodes.ContainsKey(adjacentPosition))
                {
                    node.adjacentNodes.Add(nodes[adjacentPosition]);
                }
            }
        }

        Vector3 playerPosition = PlayerSpawner.instance.spawnPosition;
        playerPosition.y -= gameObject.transform.localScale.y / 2;
        Vector3 exitPosition = ExitSpawner.instance.exitPosition;
        exitPosition.y -= gameObject.transform.localScale.y / 2;

        // Check if player and exit positions are within bounds
        if (!walkableTilePositions.Contains(playerPosition))
        {
            Debug.LogError("Player position not within bounds of walkable tiles: " + playerPosition);
            yield break;
        }

        if (!walkableTilePositions.Contains(exitPosition))
        {
            Debug.LogError("Exit position not within bounds of walkable tiles: " + exitPosition);
            yield break;
        }

        // Add adjacent nodes to player and exit positions
        Node playerNode = nodes[playerPosition];
        Node exitNode = nodes[exitPosition];

        foreach (Node node in nodes.Values)
        {
            if (node.position == playerPosition || node.position == exitPosition)
            {
                continue;
            }

            if (Mathf.Abs(node.position.x - playerPosition.x) + Mathf.Abs(node.position.y - playerPosition.y) <= 1)
            {
                playerNode.adjacentNodes.Add(node);
            }

            if (Mathf.Abs(node.position.x - exitPosition.x) + Mathf.Abs(node.position.y - exitPosition.y) <= 1)
            {
                exitNode.adjacentNodes.Add(node);
            }
        }

        List<Vector3> path = FindShortestPath(playerPosition, exitPosition);

        Debug.Log("Shortest path found: " + string.Join(", ", path));

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log("Path node: " + path[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Node
{
    public Vector3 position;
    public List<Node> adjacentNodes = new List<Node>();
}