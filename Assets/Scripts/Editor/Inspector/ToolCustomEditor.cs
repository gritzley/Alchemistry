using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Tool))]
[CanEditMultipleObjects]
public class ToolEditor : Editor
{
    // Serialized properties for recipe and name
    SerializedProperty conversionsProp;
    SerializedProperty ingredientPrefabProp;

    // Reorderable list for the recipe
    ReorderableList Conversions;

    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load properties
        conversionsProp = serializedObject.FindProperty("Conversions");
        ingredientPrefabProp = serializedObject.FindProperty("IngredientPrefab");

        // create the reorderable list
        Conversions = new ReorderableList(serializedObject, conversionsProp, true, true, true, true);

        // Define how the Unity should display elements in the list by overriding the exposed callback
        Conversions.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Get the current element
            var element = Conversions.serializedProperty.GetArrayElementAtIndex(index);

            // Increase target rect y to move the next object down on screen (rect 0,0 is in top left)
            rect.y += 2;

            float halfWidth = rect.width / 2;

            // Create a property field for the ingredient
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized property
                element.FindPropertyRelative("Input"),
                // no GUI content needed
                GUIContent.none
            );
            // Create a property field for the cooking time
            EditorGUI.PropertyField (
                // Set the rectangle for the property field position
                new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                // Get the serialized Property
                element.FindPropertyRelative("Output"),
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

        EditorGUILayout.PropertyField(ingredientPrefabProp);

        // Add the reorderable list to the layout
        Conversions.DoLayoutList();

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}