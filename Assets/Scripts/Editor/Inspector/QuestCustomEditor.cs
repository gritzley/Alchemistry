using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Quest))]
[CanEditMultipleObjects]
public class QuestEditor : Editor
{
    SerializedProperty titleProp;
    SerializedProperty linksProp;

    ReorderableList Links;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load serialized properties
        titleProp = serializedObject.FindProperty("Title");
        linksProp = serializedObject.FindProperty("Links");

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
            EditorGUILayout.PropertyField(nextQuestProp, new GUIContent(potion.name));

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