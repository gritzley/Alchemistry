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
        // Load serialized properties
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
            Potion potion = (Potion)element.FindPropertyRelative("Potion").objectReferenceValue;
            SerializedProperty nextLine = element.FindPropertyRelative("NextLine");
            EditorGUILayout.PropertyField(nextLine, new GUIContent(potion.name));
        }

        // Apply all changes made to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}