using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PotionBranch))]
[CanEditMultipleObjects]
public class PotionBranchEditor : Editor
{
    SerializedProperty linksProp;
    ReorderableList Links;
    
    // This gets called when the inspector is opened
    void OnEnable()
    {
        linksProp = serializedObject.FindProperty("Links");
        (target as PotionBranch).UpdateLinks();
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized Object. Always do this before working with the object
        serializedObject.Update();

        for (int i = 0 ; i < linksProp.arraySize; i++)
        {
            SerializedProperty element = linksProp.GetArrayElementAtIndex(i);
            PotionDefinition potion = (PotionDefinition)element.FindPropertyRelative("Potion").objectReferenceValue;
            SerializedProperty nextNode = element.FindPropertyRelative("NextNode");
            EditorGUILayout.PropertyField(nextNode, new GUIContent(potion.name));
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}