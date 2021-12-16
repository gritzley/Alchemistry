using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

[CustomEditor(typeof(Potion))]
[CanEditMultipleObjects]
public class PotionEditor : Editor
{
    // Serialized properties for recipe and name
    SerializedProperty recipeProp;
    SerializedProperty nameProp;

    // Reorderable list for the recipe
    ReorderableList Recipe;

    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load properties
        recipeProp = serializedObject.FindProperty("Recipe");

        // create the reorderable list
        Recipe = new ReorderableList(serializedObject, recipeProp, true, true, true, true);

        // Define how the Unity should display elements in the list by overriding the exposed callback
        Recipe.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Get the current element
            var element = Recipe.serializedProperty.GetArrayElementAtIndex(index);

            // Increase target rect y to move the next object down on screen (rect 0,0 is in top left)
            rect.y += 2;

            float thirdWidth = rect.width / 3;

            // Create a property field for the ingredient
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized property
                element.FindPropertyRelative("Ingredient"),
                // no GUI content needed
                GUIContent.none
            );
            // Create a property field for the cooking time
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x + thirdWidth, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized Property
                element.FindPropertyRelative("Time"),
                // no GUI content needed
                GUIContent.none
            );
            // Create a propertyField for the error magin
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x + thirdWidth * 2, rect.y, thirdWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized Property
                element.FindPropertyRelative("ErrorMargin"),
                // no GUI content needed
                GUIContent.none
            );
        };
    }

    // This gets called every frame that the inspector is drawn
    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Add the reorderable list to the layout
        Recipe.DoLayoutList();

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}