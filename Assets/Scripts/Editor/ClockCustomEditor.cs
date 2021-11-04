using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(ClockController))]
[CanEditMultipleObjects]
public class ClockEditor : Editor
{
    SerializedProperty Hours, Minutes, Seconds;

    void OnEnable() 
    {
        Hours = serializedObject.FindProperty("Hours");
        Minutes = serializedObject.FindProperty("Minutes");
        Seconds = serializedObject.FindProperty("Seconds");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Add a property field for the name to the layout
        EditorGUILayout.PropertyField(Hours);
        EditorGUILayout.PropertyField(Minutes);
        EditorGUILayout.PropertyField(Seconds);

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}