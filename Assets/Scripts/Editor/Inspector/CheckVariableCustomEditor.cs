using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckVariableNode))]
[CanEditMultipleObjects]
public class CheckVariableCustomEditor : Editor
{
    SerializedProperty KeyProperty;
    string[] Keys;

    void OnEnable()
    {
        KeyProperty = serializedObject.FindProperty("Key");
        UpdateValues();
    }

    void UpdateValues()
    {
        Keys = SetVariableNode.AllVariables.ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int index = Keys.ToList().IndexOf(KeyProperty.stringValue);
        int newIndex = EditorGUILayout.Popup(index, Keys);
        if (index != newIndex)
        {
            KeyProperty.stringValue = Keys[newIndex];
            serializedObject.ApplyModifiedProperties();
            (serializedObject.targetObject as CheckVariableNode).UpdateOutPoints();
            UpdateValues();
        }
    }
}