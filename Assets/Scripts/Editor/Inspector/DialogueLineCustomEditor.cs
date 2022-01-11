using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogueLine))]
[CanEditMultipleObjects]
public class DialogueLineEditor : Editor
{
    // Serialized properties for recipe and name
    SerializedProperty titleProp;
    SerializedProperty notesProp;
    SerializedProperty textProp;
    SerializedProperty hasAnswersProp;
    SerializedProperty isReceivingProp;
    SerializedProperty answerLeftProp;
    SerializedProperty answerRightProp;

    SerializedProperty nextLeftProp;
    SerializedProperty nextRightProp;

    // Reorderable list for the recipe
    ReorderableList Recipe;

    void OnEnable()
    {
        titleProp = serializedObject.FindProperty("Title");
        notesProp = serializedObject.FindProperty("Notes");
        textProp = serializedObject.FindProperty("Text");
        hasAnswersProp = serializedObject.FindProperty("HasAnswers");
        isReceivingProp = serializedObject.FindProperty("IsReceivingState");
        answerLeftProp = serializedObject.FindProperty("AnswerLeft");
        answerRightProp = serializedObject.FindProperty("AnswerRight");
        nextRightProp = serializedObject.FindProperty("NextRight");
        nextLeftProp = serializedObject.FindProperty("NextLeft");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        EditorGUILayout.PropertyField(titleProp);
        EditorGUILayout.LabelField("Text");
        textProp.stringValue = EditorGUILayout.TextArea(textProp.stringValue);

        EditorGUILayout.PropertyField(hasAnswersProp);
        if (hasAnswersProp.boolValue)
        {
            EditorStyles.textField.wordWrap = true;
            EditorGUILayout.LabelField("Right Answer (This is the upper ConnectionPoint");
            answerRightProp.stringValue = EditorGUILayout.TextArea(answerRightProp.stringValue);
            EditorGUILayout.LabelField("Left Answer (This is the lower ConnectionPoint");
            answerLeftProp.stringValue = EditorGUILayout.TextArea(answerLeftProp.stringValue);
        }
        DialogueLine line = ((DialogueLine)serializedObject.targetObject);
        line.IsReceivingState = EditorGUILayout.Toggle(new GUIContent("Is Receiving Potions"), line.IsReceivingState);

        EditorGUILayout.LabelField("Notes");
        notesProp.stringValue = EditorGUILayout.TextArea(notesProp.stringValue);

        if (GUILayout.Button("Test Dialogue"))
        {
            GameManager.Instance.CurrentCustomer.Say(textProp.stringValue);
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}