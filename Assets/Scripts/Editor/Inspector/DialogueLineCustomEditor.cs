using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections;

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
        nextLeftProp = serializedObject.FindProperty("NextLeft");
        nextRightProp = serializedObject.FindProperty("NextRight");

        if (DialogueEditor.IsOpen)
        {
            EditorWindow.GetWindow<DialogueEditor>().Repaint();
        }
    }

    // This gets called every frame that the inspector is drawn
    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        // Add a property field for title and text
        EditorGUILayout.PropertyField(titleProp);
        EditorGUILayout.PropertyField(textProp);

        // Toggle for hasAnswer
        EditorGUILayout.PropertyField(hasAnswersProp);


        if(hasAnswersProp.boolValue)
        {
            EditorGUILayout.PropertyField(answerLeftProp);
            EditorGUILayout.PropertyField(nextLeftProp);
            EditorGUILayout.PropertyField(answerRightProp);
            EditorGUILayout.PropertyField(nextRightProp);
        }
        else
        {
            EditorGUILayout.PropertyField(nextRightProp, new GUIContent("Next"));
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}