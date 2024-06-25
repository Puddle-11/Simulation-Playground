using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ProceduralTerrain : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float vertexSpacing;
    [SerializeField] private MapOctave[] octave;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Material particleMat;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Vector3 scrollDir;
    [SerializeField] private Gradient terrainColors;
    private float timeS;
    private Vector3[] verticies;
    private int[] triangles;
    public Color[] colors;
    private float maxPerlinHeight;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;


    public GameObject debugObject;
    public bool updateMesh;
    [System.Serializable]
    public struct MapOctave
    {
        public float octaveScale;
        public float octaveHeight;
        public float octaveWeight;
    }

    private void Update()
    {
        if (updateMesh)
        {

            UpdateTerrain();
            updateMesh = false;
        }
        if(scrollSpeed != 0)
        {
            timeS += Time.deltaTime;
            UpdateTerrain();
        }
    }
    private void Awake()
    {
        float totalWeight = 0;
       
        for (int i = 0; i < octave.Length; i++)
        {
            totalWeight += octave[i].octaveWeight;
            maxPerlinHeight += 1 * octave[i].octaveHeight * octave[i].octaveWeight;

        }
        maxPerlinHeight /= totalWeight;
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
    }
    private void Start()
    {
        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        verticies = new Vector3[gridSize.x * gridSize.x];
        triangles = new int[(gridSize.x - 1) * (gridSize.x - 1) * 6];
        colors = new Color[verticies.Length];
        //set colors
     

        int i = 0;
        for (int y = 0; y < gridSize.x; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = new Vector3(x, 0 , y) * vertexSpacing;
                pos.y = sampleHeight(new Vector2(pos.x, pos.z));
                verticies[i] = pos;
                float colorPos = pos.y / maxPerlinHeight;
                colors[i] = terrainColors.Evaluate(colorPos);
                i++;
            }
        }
        //draw the triangles clockwise for the vector to be pointing up

        int it = 0;
        for (int xpos = 0; xpos < gridSize.x - 1; xpos++)
        {
            for (int ypos = 0; ypos < gridSize.x - 1; ypos++)
            {

                triangles[it++] = gridSize.x + xpos + (ypos * gridSize.x);
                triangles[it++] = xpos + 1 + (ypos * gridSize.x);
                triangles[it++] = xpos + 0 + (ypos * gridSize.x);
                triangles[it++] = gridSize.x + xpos + (ypos * gridSize.x);
                triangles[it++] = gridSize.x + 1 + xpos + (ypos * gridSize.x);
                triangles[it++] = xpos + 1 + (ypos * gridSize.x);
            }
        }


        UpdateMesh();
    }
    private float sampleHeight(Vector2 _pos)
    {
        float res = 0;
        float totalWeight = 0;
        Vector3 pos = new Vector3(_pos.x, 0, _pos.y) + scrollDir * scrollSpeed * timeS;

        for (int i = 0; i < octave.Length; i++)
        {
            totalWeight += octave[i].octaveWeight;
            res += Mathf.PerlinNoise(pos.x / octave[i].octaveScale, pos.z / octave[i].octaveScale) * octave[i].octaveHeight * octave[i].octaveWeight;
        }
        res /= totalWeight;
        return res;
    }
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshRenderer.material = particleMat;
    }
}
