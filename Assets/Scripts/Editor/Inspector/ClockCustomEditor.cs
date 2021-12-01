using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
// This Editor is mostly used because unity can normally not display properties with getters and setters
[CustomEditor(typeof(ClockController))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class ClockEditor : Editor
{
    // Serialized properties for hours, minutes and seconds
    int Hours, Minutes, Seconds;
    ClockController clock;

    void OnEnable() 
    {
        // Initialize clock
        clock = target as ClockController;
    }

    public override void OnInspectorGUI()
    {
        // Add a property field for the name to the layout
        Hours   = EditorGUILayout.IntField( (int) clock.Hours   );
        Minutes = EditorGUILayout.IntField( (int) clock.Minutes );
        Seconds = EditorGUILayout.IntField( (int) clock.Seconds );

        // Update the time if a new value is put into a field
        if ( Hours   != (int) clock.Hours   ) clock.Hours   = Hours;
        if ( Minutes != (int) clock.Minutes ) clock.Minutes = Minutes;
        if ( Seconds != (int) clock.Seconds ) clock.Seconds = Seconds;
    }
}