using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(Encounter))]
[CanEditMultipleObjects]
public class EncounterEditor : Editor
{
    SerializedProperty startBitProp;
    SerializedProperty linksProp;

    // Reorderable list for the Links
    ReorderableList Links;

    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load serialized properties
        startBitProp = serializedObject.FindProperty("StartBit");
        linksProp = serializedObject.FindProperty("Links");

        // create the reorderable list
        Links = new ReorderableList(serializedObject, linksProp, true, true, true, true);

        // Define how the Unity should display elements in the list by overriding the exposed callback
        Links.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Get the current element
            var element = Links.serializedProperty.GetArrayElementAtIndex(index);

            // Increase target rect y to move the next object down on screen (rect 0,0 is in top left)
            rect.y += 2;

            float quarterWidth = rect.width / 4;

            // Create a property field for the text
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x, rect.y, (quarterWidth * 2) - 5, EditorGUIUtility.singleLineHeight),
                // Get the serialized property
                element.FindPropertyRelative("Potion"),
                // no GUI content needed
                GUIContent.none
            );
            // Create a property field for the next bit
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x + quarterWidth * 2, rect.y, quarterWidth * 2, EditorGUIUtility.singleLineHeight),
                // Get the serialized Property
                element.FindPropertyRelative("NextEncounter"),
                // no GUI content needed
                GUIContent.none
            );
        };
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Create a property field for the start prop
        EditorGUILayout.PropertyField(startBitProp);

        // Add the reorderable list to the layout
        Links.DoLayoutList();

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }

}