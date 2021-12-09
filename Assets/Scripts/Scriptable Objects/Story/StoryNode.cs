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
    bool isDragging;

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

    public virtual void ProcessEvent(Event e)
    {
        switch(e.type)
        {
            // ---- ON DRAG ----
            case EventType.MouseDrag:
                if (rect.Contains(e.mousePosition))
                {
                    if (Event.current.button == 0)
                    {
                        isDragging = true;

                        // Move the position in the view
                        Position += Event.current.delta;

                        // Log window changes so we get a repaint
                        GUI.changed = true;
                    }
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
        GUI.Box(rect, "", style);
    }

}
