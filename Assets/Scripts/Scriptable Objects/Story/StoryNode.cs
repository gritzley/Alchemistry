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

    public Action<StoryNode> OnRemove;

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
            case EventType.MouseDown:
            // ---- DRAG START ----
                if (e.button == 0)
                {
                    isDragging = rect.Contains(e.mousePosition);
                    if (isDragging)
                    {
                        Selection.activeObject = this;
                    }
                }

            // ---- CONTEXT MENU ----
                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    GenericMenu contextMenu = new GenericMenu();
                    FillContextMenu(contextMenu);
                    contextMenu.ShowAsContext();
                    e.Use();
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

    /// <summary>
    /// Draws the Node on screen
    /// </summary>
    /// <param name="offset">The window offset, needed to draw the node at the correct place</param>
    /// <param name="state">The index of the current viewState. This is only relevant for overrides so we overwrite this and drop it</param>
    public virtual void Draw(Vector2 offset, int state = 0)
    {
        rect = new Rect(Position + offset, Size);
        // Draw own box
        GUI.Box(rect, "", Selection.activeObject == this ? selectedStyle : style);
    }

    /// <summary>
    /// This is where we define the content for this nodes contet menu
    /// By defautl this only adds a "Remove Node" button
    /// Overwrite this to add more buttons
    /// </summary>
    /// <param name="contextMenu">A reference to the context Menu. Fill it by calling AddItem on it</param>
    public virtual void FillContextMenu(GenericMenu contextMenu)
    {
        contextMenu.AddItem(new GUIContent("Remove Node"), false, Remove);
        GUI.changed = true;
    }

    /// <summary>
    /// Removes this Nodes Asset from the AssetDatabase and calls the OnRemove Action to handle additional OnRemove Stuff
    /// </summary>
    public virtual void Remove()
    {
        AssetDatabase.DeleteAsset($"Assets/Dialogue/{this.name}.asset");
        AssetDatabase.SaveAssets();
        OnRemove?.Invoke(this);
    }
}
