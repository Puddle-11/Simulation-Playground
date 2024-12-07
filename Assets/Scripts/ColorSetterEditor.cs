using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[CustomEditor(typeof(ColorSetter))]

public class ColorSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ColorSetter _target = (ColorSetter)target;
        if (_target == null) return;
        Undo.RecordObject(_target, "Color Setter Change");
        EditorGUI.DrawRect(new Rect(0,0, 10, 10), _target.GetColor());

        if (GUILayout.Button("Set Color"))
        {

            _target.UpdateColor();
        }
    }



}
