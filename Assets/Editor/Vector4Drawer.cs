using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(Vector4))]
public class Vector4Drawer : PropertyDrawer
{
    //public override float GetPropertyHeight(SerializedProperty property,
    //                                        GUIContent label)
    //{
    //    return EditorGUI.GetPropertyHeight(property, label, true);
    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //property.vector4Value = EditorGUI.Vector4Field(new Rect(position.x, position.y - 1, position.width, position.height),property.displayName, property.vector4Value);
        //EditorGUILayout.LabelField("");
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUIUtility.labelWidth = 95;
            EditorGUILayout.LabelField(property.displayName);
            EditorGUIUtility.labelWidth = 15; EditorGUIUtility.fieldWidth = 40;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("x"), new GUIContent(property.FindPropertyRelative("x").displayName));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("y"), new GUIContent(property.FindPropertyRelative("y").displayName));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("z"), new GUIContent(property.FindPropertyRelative("z").displayName));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("w"), new GUIContent(property.FindPropertyRelative("w").displayName));
        }
        EditorGUILayout.EndHorizontal();
    }
}
