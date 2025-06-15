using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
#if UNITY_EDITOR
[CustomEditor(typeof(DrawGizmo))]
[ExecuteAlways]
public class DrawGizmosEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawGizmo _target = (DrawGizmo)target;
        if (_target == null) return;

        Undo.RecordObject(_target, "DrawGizmo change");
        DrawFields(_target);
        SceneView.RepaintAll();
    }
    private void DrawFields(DrawGizmo _targetCast)
    {

        switch (_targetCast.drawType)
        {
            case DrawGizmo.Type.cube:
                _targetCast.offset = EditorGUILayout.Vector3Field("Offset", _targetCast.offset);

                _targetCast.size = EditorGUILayout.Vector3Field("Cube Size", _targetCast.size);
                break;
            case DrawGizmo.Type.sphere:
                _targetCast.offset = EditorGUILayout.Vector3Field("Offset", _targetCast.offset);

                _targetCast.size.x = EditorGUILayout.FloatField("Sphere Radius", _targetCast.size.x);
                break;
            case DrawGizmo.Type.mesh:
                _targetCast.localScale = EditorGUILayout.Toggle("Use Local Scale", _targetCast.localScale);
                _targetCast.offset = EditorGUILayout.Vector3Field("Offset", _targetCast.offset);

                _targetCast.size = EditorGUILayout.Vector3Field("Mesh Scale", _targetCast.size);
                _targetCast.rotation = EditorGUILayout.Vector3Field("Mesh rotation", _targetCast.rotation);

                _targetCast.collMesh = EditorGUILayout.ObjectField("Mesh", _targetCast.collMesh, typeof(Mesh), true) as Mesh;
                break;
            case DrawGizmo.Type.collider:

                _targetCast.coll = EditorGUILayout.ObjectField("Collider", _targetCast.coll, typeof(Collider), true) as Collider;
                if (_targetCast.coll != null && (_targetCast.coll.GetComponent<WheelCollider>() || _targetCast.coll.GetComponent<MeshCollider>()))
                {
                    _targetCast.rotation = EditorGUILayout.Vector3Field("Rotation", _targetCast.rotation);

                }
                break;
            case DrawGizmo.Type.circle:
                _targetCast.rotation = EditorGUILayout.Vector3Field("Rotation", _targetCast.rotation);
                _targetCast.size.x = EditorGUILayout.FloatField("Radius", _targetCast.size.x);
                _targetCast.offset = EditorGUILayout.Vector3Field("Offset", _targetCast.offset);
                break;
            case DrawGizmo.Type.hemisphere:
                _targetCast.size.x = EditorGUILayout.FloatField("Radius", _targetCast.size.x);

                break;

        }



    }

}
#endif
