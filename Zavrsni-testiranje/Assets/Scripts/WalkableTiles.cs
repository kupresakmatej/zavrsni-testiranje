using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WalkableTiles : MonoBehaviour
{
    public float maxSlopeAngle = 30f;
    public LayerMask walkableLayer;
    public float tileSize = 1f;
    public Color tileColor = Color.green;
    public MeshFilter meshFilter;

    public List<Vector3> walkableTilePositions = new List<Vector3>();

    public static WalkableTiles instance;

    private void Awake()
    {
        walkableTilePositions.Clear();

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
        GetWalkableTiles();
    }

    public void GetWalkableTiles()
    {
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        HashSet<int> drawnTriangles = new HashSet<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Check if the triangle has already been drawn
            if (drawnTriangles.Contains(i))
            {
                continue;
            }

            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            if (v1.x > v2.x) { var temp = v1; v1 = v2; v2 = temp; }
            if (v1.x > v3.x) { var temp = v1; v1 = v3; v3 = temp; }
            if (v2.x > v3.x) { var temp = v2; v2 = v3; v3 = temp; }
            if (v1.z > v2.z) { var temp = v1; v1 = v2; v2 = temp; }
            if (v1.z > v3.z) { var temp = v1; v1 = v3; v3 = temp; }
            if (v2.z > v3.z) { var temp = v2; v2 = v3; v3 = temp; }

            //calculating the normal vector of a triangle using its three vertices,
            //which is an important step in determining whether the triangle is facing upwards or downwards,
            //and whether it is walkable based on its slope
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            if (Vector3.Angle(normal, Vector3.up) <= maxSlopeAngle && ((1 << gameObject.layer) & walkableLayer) != 0)
            {
                //check if the normal is facing upward or downward
                if (normal.y >= 0f)
                {
                    Vector3 centerPosition = (v1 + v2 + v3) / 3f;
                    centerPosition -= new Vector3(tileSize / 2f, 0f, tileSize / 2f); // center the position

                    walkableTilePositions.Add(centerPosition);

                    drawnTriangles.Add(i);
                    drawnTriangles.Add(i + 1);
                    drawnTriangles.Add(i + 2);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        DrawWalkableTiles();
    }

    private void DrawWalkableTiles()
    {
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        HashSet<int> drawnTriangles = new HashSet<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Check if the triangle has already been drawn
            if (drawnTriangles.Contains(i))
            {
                continue;
            }

            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            if (v1.x > v2.x) { var temp = v1; v1 = v2; v2 = temp; }
            if (v1.x > v3.x) { var temp = v1; v1 = v3; v3 = temp; }
            if (v2.x > v3.x) { var temp = v2; v2 = v3; v3 = temp; }
            if (v1.z > v2.z) { var temp = v1; v1 = v2; v2 = temp; }
            if (v1.z > v3.z) { var temp = v1; v1 = v3; v3 = temp; }
            if (v2.z > v3.z) { var temp = v2; v2 = v3; v3 = temp; }

            //calculating the normal vector of a triangle using its three vertices,
            //which is an important step in determining whether the triangle is facing upwards or downwards,
            //and whether it is walkable based on its slope
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            if (Vector3.Angle(normal, Vector3.up) <= maxSlopeAngle && ((1 << gameObject.layer) & walkableLayer) != 0)
            {
                Gizmos.color = tileColor;

                //check if the normal is facing upward or downward
                if (normal.y >= 0f)
                {
                    Vector3 centerPosition = (v1 + v2 + v3) / 3f;
                    centerPosition -= new Vector3(tileSize / 2f, 0f, tileSize / 2f); // center the position
                    Gizmos.DrawCube(centerPosition, Vector3.one * tileSize);

                    drawnTriangles.Add(i);
                    drawnTriangles.Add(i + 1);
                    drawnTriangles.Add(i + 2);
                }
            }
        }
    }

    public List<Vector3> GetWalkableTilePositions()
    {
        return walkableTilePositions;
    }
}