using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(Vector4))]
public class Vector4Drawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.vector4Value = EditorGUI.Vector4Field(new Rect(position.x, position.y - 1, position.width, position.height),property.displayName, property.vector4Value);
        EditorGUILayout.LabelField("");
    }
}
