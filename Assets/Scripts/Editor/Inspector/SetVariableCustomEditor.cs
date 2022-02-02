using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SetVariableNode))]
[CanEditMultipleObjects]
public class SetVariableCustomEditor : Editor
{
    SerializedProperty keyProperty;
    SerializedProperty valueProperty;
    string[] Variables;
    bool createKeyMenuOpen = false;
    string newKey;

    void OnEnable()
    {
        keyProperty = serializedObject.FindProperty("Key");
        valueProperty = serializedObject.FindProperty("Value");
        UpdateVariables();
    }
    void UpdateVariables()
    {
        Variables = SetVariableNode.AllVariables.Prepend("Create new Key").ToArray();
    }
    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        if (createKeyMenuOpen)
        {
            newKey = EditorGUILayout.TextField(newKey);
            if (GUILayout.Button("Create Key"))
            {
                keyProperty.stringValue = newKey;
                newKey = String.Empty;
                createKeyMenuOpen = false;
                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
                UpdateVariables();
            }
        }
        else
        {
            int index = Variables.ToList().IndexOf(keyProperty.stringValue);
            int newIndex = EditorGUILayout.Popup(new GUIContent("Select Key"), index, Variables);

            if (newIndex == 0) createKeyMenuOpen = true;
            else if (index != newIndex) keyProperty.stringValue = Variables[newIndex];
        }
        EditorGUILayout.PropertyField(valueProperty);
        serializedObject.ApplyModifiedProperties();

    }
}