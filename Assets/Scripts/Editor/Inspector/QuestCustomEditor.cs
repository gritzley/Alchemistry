using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Quest))]
[CanEditMultipleObjects]
public class QuestEditor : Editor
{
    SerializedProperty titleProp;
    SerializedProperty linksProp;
    SerializedProperty linesProp;

    ReorderableList Links;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load serialized properties
        titleProp = serializedObject.FindProperty("Title");
        linksProp = serializedObject.FindProperty("Links");
        linesProp = serializedObject.FindProperty("Lines");

        (target as Quest).UpdateLinks();
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Create a property field for the start prop
        EditorGUILayout.PropertyField(titleProp);

        for (int i = 0 ; i < linksProp.arraySize; i++)
        {
            SerializedProperty element = linksProp.GetArrayElementAtIndex(i);
            Potion potion = (Potion)element.FindPropertyRelative("Potion").objectReferenceValue;
            SerializedProperty nextQuestProp = element.FindPropertyRelative("NextQuest");
            EditorGUILayout.PropertyField(nextQuestProp, new GUIContent(potion.Name));

            if (nextQuestProp.objectReferenceValue == target)
            {
                nextQuestProp.objectReferenceValue = null;
                Debug.LogWarning("Quests can not refer to themselves");
            }
        }

        EditorGUILayout.PropertyField(linesProp);

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }

    

}