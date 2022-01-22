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
    SerializedProperty nextProp;
    SerializedProperty customerProp;

    ReorderableList Links;
    List<SceneNode> relatedScenes;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        titleProp = serializedObject.FindProperty("Title");
        linksProp = serializedObject.FindProperty("Links");
        nodesProp = serializedObject.FindProperty("DialogueNodes");
        customerProp = serializedObject.FindProperty("Customer");
        nextProp = serializedObject.FindProperty("NextScene");

        (target as Quest).UpdateLinks();
        CustomerDefinition customer = ((Quest)serializedObject.targetObject).Customer;
        relatedScenes = new List<SceneNode>();
        relatedScenes.Add(null);
        relatedScenes.AddRange(customer.Quests);
        relatedScenes.AddRange(customer.Articles);
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Create a property field for the start prop
        EditorGUILayout.PropertyField(titleProp);
        EditorGUILayout.PropertyField(customerProp);
        EditorGUILayout.PropertyField(nodesProp);

        if ((serializedObject.targetObject as Quest).HasReceivingState)
        {
            for (int i = 0 ; i < linksProp.arraySize; i++)
            {
                SerializedProperty element = linksProp.GetArrayElementAtIndex(i);
                PotionDefinition potion = (PotionDefinition)element.FindPropertyRelative("Potion").objectReferenceValue;
                SerializedProperty nextQuestProp = element.FindPropertyRelative("NextQuest");

                int questIndex = relatedScenes.IndexOf((SceneNode)nextQuestProp.objectReferenceValue);
                int newQuestIndex = EditorGUILayout.Popup(new GUIContent(potion.name), questIndex, relatedScenes.Select(e => e ? e.Title:"None").ToArray());
                
                if (questIndex != newQuestIndex)
                {
                    nextQuestProp.objectReferenceValue = relatedScenes[newQuestIndex];
                    StoryEditor.Instance?.Repaint();
                }

                // EditorGUILayout.PropertyField(nextQuestProp, new GUIContent(potion.name));

                if (nextQuestProp.objectReferenceValue == target)
                {
                    nextQuestProp.objectReferenceValue = null;
                    Debug.LogWarning("Quests can not refer to themselves");
                }
            }
        }
        else
        {
            int sceneIndex = relatedScenes.IndexOf((SceneNode)nextProp.objectReferenceValue);
            int newSceneIndex = EditorGUILayout.Popup(new GUIContent("Next Scene"), sceneIndex, relatedScenes.Select(e => e ? e.Title : "None").ToArray());
            
            if (sceneIndex != newSceneIndex)
            {
                nextProp.objectReferenceValue = relatedScenes[newSceneIndex];
                StoryEditor.Instance?.Repaint();
            }

            // EditorGUILayout.PropertyField(nextProp, new GUIContent(potion.name));

            if (nextProp.objectReferenceValue == target)
            {
                nextProp.objectReferenceValue = null;
                Debug.LogWarning("Quests can not refer to themselves");
            }
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}