using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Test : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Mesh m;
    public Vector3[] verts;
    public int[] tris;
    public Vector2Int boardSize;
    public float noiseScale;
    public float cellSize;
    public float noiseStrength;
    public float scrollSpeed;
    public Vector2 scrolldir;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        m = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        SetBoard();
    }
    public void SetBoard()
    {
        verts = new Vector3[boardSize.x* boardSize.y];
        tris = new int[(boardSize.x -1) * (boardSize.y - 1) * 2];
        tris = new int[3];
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                float height = SampleNoise(BoardToWorld(new Vector2Int(x,y)));
                Vector3 xz = BoardToWorld(new Vector2Int(x,y));
          
                verts[x + (y * boardSize.x)] = xz + Vector3.up * height + transform.position;
            }
        }
        tris[2] = 0;
        tris[1] = 1;
        tris[0] = boardSize.x;
        DrawMesh();
    }
    public float SampleNoise(Vector3 _pos)
    {
        _pos /= noiseScale;
        _pos += new Vector3(scrolldir.x, 0, scrolldir.y) * scrollSpeed * timer;
        return Mathf.PerlinNoise(_pos.x, _pos.z) * noiseStrength;

    }
    public Vector3 BoardToWorld(Vector2Int _pos)
    {
        return new Vector3(_pos.x, 0, _pos.y) * cellSize;
    }
    private void DrawMesh()
    {
        m.vertices = verts;
        m.triangles = tris;

        meshFilter.mesh = m;


    }
    private void OnDrawGizmos()
    {
        if (verts == null || verts.Length == 0) return;
        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }
}
