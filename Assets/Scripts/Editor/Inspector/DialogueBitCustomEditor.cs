using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(DialogueBit))]
[CanEditMultipleObjects]
public class DialogueBitEditor : Editor
{
    SerializedProperty nameProp;
    SerializedProperty textProp;

    SerializedProperty choicesProp;

    // Reorderable list for the Choices
    ReorderableList Choices;

    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load properties
        textProp = serializedObject.FindProperty("Text");
        choicesProp = serializedObject.FindProperty("Choices");

        // create the reorderable list
        Choices = new ReorderableList(serializedObject, choicesProp, true, true, true, true);

        // Define how the Unity should display elements in the list by overriding the exposed callback
        Choices.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Get the current element
            var element = Choices.serializedProperty.GetArrayElementAtIndex(index);

            // Increase target rect y to move the next object down on screen (rect 0,0 is in top left)
            rect.y += 2;

            float quarterWidth = rect.width / 4;

            // Create a property field for the text
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x, rect.y, (quarterWidth * 3) - 5, EditorGUIUtility.singleLineHeight),
                // Get the serialized property
                element.FindPropertyRelative("Text"),
                // no GUI content needed
                GUIContent.none
            );
            // Create a property field for the next bit
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x + quarterWidth * 3, rect.y, quarterWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized Property
                element.FindPropertyRelative("Next"),
                // no GUI content needed
                GUIContent.none
            );
        };
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Add a property field for the name to the layout
        EditorGUILayout.LabelField("Text");
        // For the text property we need a Textarea which is written a bit awkwardly
        textProp.stringValue = EditorGUILayout.TextArea(textProp.stringValue);

        // Add the reorderable list to the layout
        Choices.DoLayoutList();

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }

}