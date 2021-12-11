using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class StoryNode : ScriptableObject
{
    GUIStyle style, selectedStyle;
    bool isDragging;
    // Size and Position in Editor
    public Vector2 Position, Size;
    public GUIStyle LabelStyle;
    public Rect rect;
    public bool isSelected;
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
            // ---- MOUSE CLICKS ----
            case EventType.MouseDown:
                if (rect.Contains(e.mousePosition))
                {
                    isSelected = true;
                    OnClick();
                    if (e.button == 0)
                    {
                        isDragging = true;
                    }
                    e.Use();
                }
                else if (isSelected)
                {
                    isSelected = false;
                    GUI.changed = true;
                }
                break;

            // ---- ON DRAG ----
            case EventType.MouseDrag:
                if (isDragging)
                {
                    isDragging = true;

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
        GUI.Box(rect, "", isSelected ? selectedStyle : style);
    }

    public virtual void OnClick() { }
}
