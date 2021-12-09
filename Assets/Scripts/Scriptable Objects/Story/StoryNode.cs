using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class StoryNode : ScriptableObject
{
    // Size and Position in Editor
    public Vector2 Position, Size;

    public GUIStyle LabelStyle;
    public Rect rect;

    GUIStyle style;

    public StoryNode() {
        
    }

    public virtual void OnEnable()
    {
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);

        LabelStyle = new GUIStyle();
        LabelStyle.alignment = TextAnchor.UpperCenter;
        LabelStyle.padding = new RectOffset(30, 30, 10, 0);
        LabelStyle.normal.textColor = Color.white;
    }
    public virtual void Draw(Vector2 offset)
    {
        rect = new Rect(Position + offset, Size);
        // Draw own box
        GUI.Box(rect, "", style);
    }

}
