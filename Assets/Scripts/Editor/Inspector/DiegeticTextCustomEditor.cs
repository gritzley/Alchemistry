using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DiegeticText))]
[CanEditMultipleObjects]
public class DiegeticTextEditor : Editor
{
    SerializedProperty textProperty;
    SerializedProperty fontProperty;
    SerializedProperty fontSizeProperty;
    SerializedProperty canvasProperty;
    SerializedProperty textSpeedProperty;
    SerializedProperty lineSpacingProperty;
    SerializedProperty letterSpacingProperty;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        textProperty = serializedObject.FindProperty("Text");
        fontProperty = serializedObject.FindProperty("Font");
        fontSizeProperty = serializedObject.FindProperty("FontSize");
        canvasProperty = serializedObject.FindProperty("Canvas");
        lineSpacingProperty = serializedObject.FindProperty("lineSpacing");
        textSpeedProperty = serializedObject.FindProperty("textSpeed");
        letterSpacingProperty = serializedObject.FindProperty("letterSpacing");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(canvasProperty);
        EditorGUILayout.PropertyField(fontProperty);
        EditorGUILayout.PropertyField(fontSizeProperty);
        EditorGUILayout.PropertyField(textProperty);
        EditorGUILayout.PropertyField(lineSpacingProperty);
        EditorGUILayout.PropertyField(textSpeedProperty);
        EditorGUILayout.PropertyField(letterSpacingProperty);
        
        if (GUILayout.Button("Update Text"))
            (serializedObject.targetObject as DiegeticText).DisplayText(textProperty.stringValue);

        serializedObject.ApplyModifiedProperties();
    }
}