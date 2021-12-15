using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogueLine))]
[CanEditMultipleObjects]
public class DialogueLineEditor : Editor
{
    // Serialized properties for recipe and name
    SerializedProperty titleProp;
    SerializedProperty textProp;
    SerializedProperty hasAnswersProp;
    SerializedProperty answerLeftProp;
    SerializedProperty answerRightProp;

    SerializedProperty nextLeftProp;
    SerializedProperty nextRightProp;

    // Reorderable list for the recipe
    ReorderableList Recipe;

    // This gets called when the inspector is opened
    void OnEnable()
    {
        // Load properties
        titleProp = serializedObject.FindProperty("Title");
        textProp = serializedObject.FindProperty("Text");
        hasAnswersProp = serializedObject.FindProperty("HasAnswers");
        answerLeftProp = serializedObject.FindProperty("AnswerLeft");
        answerRightProp = serializedObject.FindProperty("AnswerRight");
        nextRightProp = serializedObject.FindProperty("NextRight");
        nextLeftProp = serializedObject.FindProperty("NextLeft");

    }

    // This gets called every frame that the inspector is drawn
    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Add a property field for title and text
        EditorGUILayout.PropertyField(titleProp);
        textProp.stringValue = EditorGUILayout.TextArea(textProp.stringValue);

        bool hasAnswerPreviousValue = hasAnswersProp.boolValue;

        // Toggle for hasAnswer
        EditorGUILayout.PropertyField(hasAnswersProp);
        EditorGUILayout.PropertyField(nextLeftProp);
        EditorGUILayout.PropertyField(nextRightProp);


        if (hasAnswersProp.boolValue)
        {
            EditorStyles.textField.wordWrap = true;
            answerRightProp.stringValue = EditorGUILayout.TextArea(answerRightProp.stringValue);
            answerLeftProp.stringValue = EditorGUILayout.TextArea(answerLeftProp.stringValue);
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}