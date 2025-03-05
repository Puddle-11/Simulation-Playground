using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ConeMeshGen : MonoBehaviour
{
    private float _radius;
    public float radius
    {

        get { return _radius; }

        set
        {
            _radius = Mathf.Clamp(value, 0.1f, Mathf.Infinity);
            BakeMesh();
        }
    }
    private float _depth;
    public float depth
    {
        get { return _depth; }
        set
        {
            _depth = Mathf.Clamp(value, 0.1f, Mathf.Infinity);
            BakeMesh();
        }

    }
    private int _resolution;
    public int resolution
    {

        get { return _resolution; }
        set
        {
            _resolution = (int)Mathf.Clamp(value, 3, 80);
            BakeMesh();
        }
    }
    private Vector2 _scale;
    public Vector2 scale
    {

        get
        {
            return _scale;
        }
        set
        {
            _scale = new Vector2(Mathf.Clamp(value.x, 0.001f, Mathf.Infinity), Mathf.Clamp(value.y, 0.001f, Mathf.Infinity));
            BakeMesh();

        }
    }
    private Vector3[] points; //points in local position (of object)
    private Vector3[] verts; //verticies of all triangles in local position (contains duplicate verticies for better uv wrapping in edge cases)
    private int[] tris; //all triangle indicies
    private MeshFilter rend;
    private MeshCollider coll;

    private void Awake()
    {
        rend = GetComponent<MeshFilter>();
        coll = GetComponent<MeshCollider>();
        coll.convex = true;
    }
    public void BakeMesh()
    {
        CalculatePoints();
        CalculateVerticies();
        CalculateTriangles();
        UpdateMesh();
    }

    public void CalculatePoints()
    {
        points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float radStep = (Mathf.PI * 2) * ((float)i / resolution);

            points[i] = new Vector3(Mathf.Sin(radStep) * scale.x, 0, Mathf.Cos(radStep) * scale.y) * radius - new Vector3(0, depth, 0);

        }
    }
    public void CalculateVerticies()
    {
        List<Vector3> tempVerticies = new List<Vector3>();

        int p = Mathf.CeilToInt((float)points.Length / 2);
        for (int i = 0; i < points.Length; i++)
        {
            //top point to bottom of circle
            tempVerticies.Add(points[i]);
            tempVerticies.Add(new Vector3(0, 0, 0));
            tempVerticies.Add(points[(i == 0 ? points.Length : i) - 1]);
            //==========================================================
            if (i >= 1)
            {
                //Bottom of circle
                if (i <= points.Length / 2)
                {
                    tempVerticies.Add(points[points.Length - i]);
                    tempVerticies.Add(points[i + 1]);
                    tempVerticies.Add(points[i]);
                }
                if (i < p)
                {

                    int startVert = points.Length - i;
                    int endVert = (startVert + 1) % points.Length;
                    tempVerticies.Add(points[startVert]);
                    tempVerticies.Add(points[i]);
                    tempVerticies.Add(points[endVert]);
                }
                //==========================================================
            }
        }

        verts = tempVerticies.ToArray();
    }

    public void CalculateTriangles()
    {

        tris = new int[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            tris[i] = i;
        }


    }
    public void UpdateMesh()
    {
        if (rend.sharedMesh == null)
        {
            rend.sharedMesh = new Mesh();
        }
        rend.sharedMesh.name = "Custom Cone";
        rend.sharedMesh.triangles = new int[tris.Length];
        rend.sharedMesh.SetVertices(verts);
        rend.sharedMesh.SetTriangles(tris, 0);
        rend.sharedMesh.RecalculateBounds();
        rend.sharedMesh.RecalculateNormals();
        if (verts.Length != 0 && tris.Length != 0)
        {
            coll.sharedMesh = rend.sharedMesh;
        }
    }




}
