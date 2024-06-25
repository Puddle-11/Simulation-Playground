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
    [SerializeField] private VectorField vf1;

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float vertexSpacing;
    [SerializeField] private MapOctave[] octave;
    private Vector3 offset;
    [SerializeField] private Material particleMat;
    [SerializeField] private Gradient terrainColors;
    private Vector3[] verticies;
    private int[] triangles;
    public Color[] colors;
    private float maxPerlinHeight;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private Vector3[] bakedVertex;

    public GameObject debugObject;
    public bool updateMesh;
    public bool continuousUpdate;
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
            BakeHeight();
            UpdateTerrain();
            updateMesh = false;
        }
    
  
        if (continuousUpdate)
        {
            UpdateTerrain();
        }
    }

    private void Awake()
    {
        offset = new Vector3(1,0,1) * gridSize.x * vertexSpacing * 0.5f * -1; 
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
        BakeHeight();

    }
    private void Start()
    {
        UpdateTerrain();
    }
    private void BakeHeight()
    {

        bakedVertex = new Vector3[gridSize.x * gridSize.x];
        triangles = new int[(gridSize.x - 1) * (gridSize.x - 1) * 6];

        for (int i = 0, y = 0; y < gridSize.x; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = new Vector3(x, sampleHeight(new Vector2(x, y)), y) * vertexSpacing + offset;
                bakedVertex[i] = pos;

                i++;
            }
        }
        for (int it = 0, xpos = 0; xpos < gridSize.x - 1; xpos++)
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
    }
    public void UpdateTerrain()
    {
        verticies = new Vector3[bakedVertex.Length];

        colors = new Color[bakedVertex.Length];
        //set colors
     

        for (int i = 0, y = 0; y < gridSize.x; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = bakedVertex[i];
                pos += vf1.SampleField(pos);
                verticies[i] = pos;
                /*
                Vector3 pos = new Vector3(x, 0 , y) * vertexSpacing;
                pos.y = sampleHeight(new Vector2(pos.x, pos.z));
                pos += offset;
                verticies[i] = pos;
                */
                float colorPos = pos.y / maxPerlinHeight;
                colors[i] = terrainColors.Evaluate(colorPos);
                i++;
            }
        }
        //draw the triangles clockwise for the vector to be pointing up

   



        UpdateMesh();
    }
    private float sampleHeight(Vector2 _pos)
    {
        float res = 0;
        float totalWeight = 0;

        for (int i = 0; i < octave.Length; i++)
        {
            totalWeight += octave[i].octaveWeight;
            res += Mathf.PerlinNoise(_pos.x / octave[i].octaveScale, _pos.y / octave[i].octaveScale) * octave[i].octaveHeight * octave[i].octaveWeight;
        }
        return res /= totalWeight;
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
