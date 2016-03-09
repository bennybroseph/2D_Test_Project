using UnityEngine;
using UnityEditor;
using Bennybroseph.MySystem;

[CustomPropertyDrawer(typeof(IntVector2))]
public class IntVector2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Debug.Log("?");
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUIUtility.labelWidth = 95;
            EditorGUILayout.LabelField(property.displayName);
            EditorGUIUtility.labelWidth = 15; EditorGUIUtility.fieldWidth = 40;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_x"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_y"), new GUIContent(property.FindPropertyRelative("m_y").displayName));
        }
        EditorGUILayout.EndHorizontal();

    }
}
