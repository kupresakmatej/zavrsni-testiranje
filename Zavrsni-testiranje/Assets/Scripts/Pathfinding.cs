using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();

    private void Start()
    {
        StartCoroutine(DijkstraFindPath());
    }

    private IEnumerator DijkstraFindPath()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(6f);

        // Create nodes for all walkable tile positions
        List<Vector3> walkableTilePositions = WalkableTiles.instance.walkableTilePositions;
        foreach (Vector3 position in walkableTilePositions)
        {
            Node node = new Node(position);
            nodes[position] = node;
        }

        // Connect nodes that are adjacent to each other
        foreach (Node node in nodes.Values)
        {
            foreach (Vector3 direction in new Vector3[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back })
            {
                Vector3 neighborPosition = node.position + direction * WalkableTiles.instance.tileSize;
                if (nodes.ContainsKey(neighborPosition))
                {
                    Node neighbor = nodes[neighborPosition];
                    if (!Physics.Linecast(node.position, neighbor.position, WalkableTiles.instance.walkableLayer))
                    {
                        float distance = Vector3.Distance(node.position, neighbor.position);
                        node.neighbors.Add(new Edge(neighbor, distance));
                        neighbor.neighbors.Add(new Edge(node, distance));
                    }
                }
            }
        }

        // Find path and debug log positions
        Vector3 start = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
        Vector3 end = new Vector3(GameObject.FindGameObjectWithTag("Exit").transform.position.x, GameObject.FindGameObjectWithTag("Exit").transform.position.y, GameObject.FindGameObjectWithTag("Exit").transform.position.z);
        List<Vector3> path = FindPath(start, end);
        if (path != null)
        {
            Debug.Log("Path positions:");
            foreach (Vector3 position in path)
            {
                Debug.Log(position);
            }
        }
        else
        {
            Debug.Log("No path found!");
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        if (!nodes.ContainsKey(start) || !nodes.ContainsKey(end))
        {
            Debug.LogError("Start or end position not found in nodes dictionary");
            return null;
        }

        Node startNode = nodes[start];
        Node endNode = nodes[end];

        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, float> distances = new Dictionary<Node, float>();
        Dictionary<Node, Node> previousNodes = new Dictionary<Node, Node>();

        distances[startNode] = 0f;

        List<Node> unvisitedNodes = new List<Node>() { startNode };

        while (unvisitedNodes.Count > 0)
        {
            Node currentNode = GetNodeWithSmallestDistance(unvisitedNodes, distances);
            unvisitedNodes.Remove(currentNode);

            if (currentNode == endNode)
            {
                return BuildPath(previousNodes, endNode);
            }

            visited.Add(currentNode);

            foreach (Edge edge in currentNode.neighbors)
            {
                Node neighbor = edge.node;
                if (!visited.Contains(neighbor))
                {
                    float tentativeDistance = distances[currentNode] + edge.cost;
                    if (!distances.ContainsKey(neighbor) || tentativeDistance < distances[neighbor])
                    {
                        distances[neighbor] = tentativeDistance;
                        previousNodes[neighbor] = currentNode;
                        unvisitedNodes.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private Node GetNodeWithSmallestDistance(List<Node> nodes, Dictionary<Node, float> distances)
    {
        Node smallestNode = nodes[0];
        foreach (Node node in nodes)
        {
            if (distances.ContainsKey(node) && distances[node] < distances[smallestNode])
            {
                smallestNode = node;
            }
        }
        return smallestNode;
    }

    private List<Vector3> BuildPath(Dictionary<Node, Node> previousNodes, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;
        while (previousNodes.ContainsKey(currentNode))
        {
            path.Add(currentNode.position);
            currentNode = previousNodes[currentNode];
        }
        path.Reverse();
        return path;
    }

    private class Node
    {
        public Vector3 position;
        public List<Edge> neighbors = new List<Edge>();

        public int distanceFromStart = int.MaxValue;
        public bool visited = false;
        public Node previous = null;

        public Node(Vector3 position)
        {
            this.position = position;
        }

        public void AddNeighbor(Node neighborNode, float cost)
        {
            Edge edge = new Edge(neighborNode, cost);
            neighbors.Add(edge);
        }
    }

    private class Edge
    {
        public Node node;
        public float cost;

        public Edge(Node node, float cost)
        {
            this.node = node;
            this.cost = cost;
        }
    }
}