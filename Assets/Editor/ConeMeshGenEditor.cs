using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CustomEditor(typeof(ConeMeshGen))]
public class ConeMeshGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ConeMeshGen _target = (ConeMeshGen)target;
        if (target == null) return;
        Undo.RecordObject(_target, "Change Cone Mesh Generator Editor");


        float newRadius = EditorGUILayout.FloatField("Radius", _target.radius);
        if (newRadius != _target.radius)  _target.radius = newRadius;


        float newDepth = EditorGUILayout.FloatField("Depth", _target.depth);
        if (newDepth != _target.depth) _target.depth = newDepth;

        int newResolution = EditorGUILayout.IntField("Resolution", _target.resolution);
        if (newResolution != _target.resolution) _target.resolution = newResolution;

        Vector2 newScale = EditorGUILayout.Vector2Field("Scale", _target.scale);
        if (newScale != _target.scale) _target.scale = newScale;


    }
}
