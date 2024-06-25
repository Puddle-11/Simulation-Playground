using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class VectorCube : MonoBehaviour
{
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    [SerializeField] private Material meshMat;
    [SerializeField] private int resolution;
    [SerializeField] private Vector2 size;
    private Vector3[] originVertices;
    private int[] triangles;
    [SerializeField] private GameObject debugPoint;
    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {

        UpdateGeometry();
    }
    private void UpdateGeometry()
    {
        originVertices = new Vector3[resolution * resolution];
        triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                originVertices[i] = new Vector3(x * size.x / resolution, 0, y * size.y / resolution);
                i++;
            }
        }

        for (int it = 0, xpos = 0; xpos < resolution - 1; xpos++)
        {
            for (int ypos = 0; ypos < resolution - 1; ypos++)
            {

                triangles[it++] = resolution + xpos + (ypos * resolution);
                triangles[it++] = xpos + 1 + (ypos * resolution);
                triangles[it++] = xpos + 0 + (ypos * resolution);
                triangles[it++] = resolution + xpos + (ypos * resolution);
                triangles[it++] = resolution + 1 + xpos + (ypos * resolution);
                triangles[it++] = xpos + 1 + (ypos * resolution);
            }
        }


        updateMesh();
    }
    private void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = originVertices;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshRenderer.material = meshMat;
    }


}
