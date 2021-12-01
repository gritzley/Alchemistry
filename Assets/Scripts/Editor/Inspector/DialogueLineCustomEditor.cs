using UnityEditor;
using UnityEditorInternal;

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

        bool hasAnswerPreviousValue = hasAnswersProp.boolValue;

        // Toggle for hasAnswer
        EditorGUILayout.PropertyField(hasAnswersProp);


        if (hasAnswersProp.boolValue)
        {
            EditorGUILayout.PropertyField(answerLeftProp);
            EditorGUILayout.PropertyField(answerRightProp);
        }
        if (hasAnswerPreviousValue != hasAnswersProp.boolValue && DialogueEditor.IsOpen)
        {
            EditorWindow.GetWindow<DialogueEditor>().Repaint();
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}