using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(Quest))]
[CanEditMultipleObjects]
public class QuestEditor : Editor
{
    SerializedProperty titleProp;
    SerializedProperty linksProp;
    SerializedProperty nodesProp;
    SerializedProperty customerProp;

    ReorderableList Links;
    List<Quest> relatedQuests;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        titleProp = serializedObject.FindProperty("Title");
        linksProp = serializedObject.FindProperty("Links");
        nodesProp = serializedObject.FindProperty("DialogueNodes");
        customerProp = serializedObject.FindProperty("Customer");

        (target as Quest).UpdateLinks();
        relatedQuests = ((Quest)serializedObject.targetObject).Customer.Quests.Prepend(null).ToList();
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Create a property field for the start prop
        EditorGUILayout.PropertyField(titleProp);
        EditorGUILayout.PropertyField(customerProp);
        EditorGUILayout.PropertyField(nodesProp);


        for (int i = 0 ; i < linksProp.arraySize; i++)
        {
            SerializedProperty element = linksProp.GetArrayElementAtIndex(i);
            Potion potion = (Potion)element.FindPropertyRelative("Potion").objectReferenceValue;
            SerializedProperty nextQuestProp = element.FindPropertyRelative("NextQuest");

            int questIndex = relatedQuests.IndexOf((Quest)nextQuestProp.objectReferenceValue);
            int newQuestIndex = EditorGUILayout.Popup(new GUIContent(potion.name), questIndex, relatedQuests.Select(e => e ? e.Title:"None").ToArray());
            
            if (questIndex != newQuestIndex)
            {
                nextQuestProp.objectReferenceValue = relatedQuests[newQuestIndex];
                StoryEditor.Instance?.Repaint();
            }

            // EditorGUILayout.PropertyField(nextQuestProp, new GUIContent(potion.name));

            if (nextQuestProp.objectReferenceValue == target)
            {
                nextQuestProp.objectReferenceValue = null;
                Debug.LogWarning("Quests can not refer to themselves");
            }
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}