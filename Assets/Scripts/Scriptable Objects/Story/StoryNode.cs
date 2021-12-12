using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class StoryNode : ScriptableObject
{
    GUIStyle style, selectedStyle;
    // Size and Position in Editor
    public Vector2 Position, Size;
    public GUIStyle LabelStyle;
    public Rect rect;
    public bool isDragging;
    public string Title;
    public abstract List<Connection> Connections { get; }

    public StoryNode() {
        
    }

    public virtual void OnEnable()
    {
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);

        selectedStyle = new GUIStyle();
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedStyle.border = new RectOffset(12, 12, 12, 12);

        LabelStyle = new GUIStyle();
        LabelStyle.alignment = TextAnchor.UpperCenter;
        LabelStyle.padding = new RectOffset(20, 20, 10, 0);
        LabelStyle.normal.textColor = Color.white;
    }

    public virtual void ProcessEvent(Event e)
    {
        switch(e.type)
        {
            // ---- DAG START ----
            case EventType.MouseDown:
                isDragging = rect.Contains(e.mousePosition);
                if (isDragging)
                {
                    Selection.activeObject = this;
                }
                break;

            // ---- ON DRAG ----
            case EventType.MouseDrag:
                if (isDragging)
                {
                    // Move the position in the view
                    Position += Event.current.delta;

                    // Log window changes so we get a repaint
                    GUI.changed = true;
                    e.Use();
                }
                break;

            // ---- DRAG END ----
            case EventType.MouseUp:
                if (isDragging)
                {
                    isDragging = false;
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                }
                break;
        }
    }
    public virtual void Draw(Vector2 offset)
    {
        rect = new Rect(Position + offset, Size);
        // Draw own box
        GUI.Box(rect, "", Selection.activeObject == this ? selectedStyle : style);
    }
}
