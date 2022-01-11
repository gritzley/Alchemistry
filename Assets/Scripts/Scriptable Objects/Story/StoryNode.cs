using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class StoryNode : ScriptableObject
{
    GUIStyle style, selectedStyle;
    public Vector2 Position, Size;
    public GUIStyle LabelStyle;
    public Rect rect;
    public bool isDragging;
    public string Title;
    public Action<StoryNode> OnRemove;

    public ConnectionPoint InPoint;

    /// <summary>
    /// Set Initial Values when Enabling the Nodes.
    /// Used to set all the styles
    /// </summary>
    public virtual void OnEnable()
    {
        // This is the default style of a node
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);

        // This is the style used for a selected Node
        selectedStyle = new GUIStyle();
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedStyle.border = new RectOffset(12, 12, 12, 12);

        // The Styl with which Labels are written
        LabelStyle = new GUIStyle();
        LabelStyle.alignment = TextAnchor.UpperCenter;
        LabelStyle.padding = new RectOffset(20, 20, 10, 0);
        LabelStyle.normal.textColor = Color.white;

        InPoint = new ConnectionPoint(this, ConnectionPointType.In, OnInPointClick, 0);
    }

    /// <summary>
    /// Process a window event
    /// </summary>
    /// <param name="e">The event</param>
    /// <param name="state">The current state of the window. 0 -> QuestView , 1 -> DialogueView</param>
    public virtual void ProcessEvent(Event e, int state = 0)
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
                if (isDragging && e.button == 0)
                {
                    Position += Event.current.delta;
                    GUI.changed = true; // Tell Unity to redraw the window
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
    /// This is called when the InPoint is clicked.
    /// </summary>
    /// <param name="i">The index of the InNode. The Field on COnnectionPoint
    /// requires an int input but we don't need one for this type so we set it to 0 and drop it</param>
    public void OnInPointClick(int i = 0)
    {
        ConnectionPoint outPoint = ConnectionPoint.selectedOutPoint;
        if (outPoint != null)
        {
            // This way we just reuse the behaviour we already coded in OnOutPointClick
            ConnectionPoint.selectedInPoint = InPoint;
            outPoint.Parent.OnOutPointClick(outPoint.Index);
        }
        else
        {
            ConnectionPoint.selectedInPoint = InPoint;
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

    /// <summary>
    /// Action passed to an ConnectionPoint for OnClick Handling
    /// </summary>
    /// <param name="index">The index of the point in this object</param>
    public abstract void OnOutPointClick(int index);

    /// <summary>
    /// Get a list of all outgoing connections from this node.
    /// Combining the connections of all nodes gives you a list of all connections
    /// </summary>
    /// <param name="state">The current state of the window. 0 -> QuestView , 1 -> DialogueView</param>
    /// <returns>All Out Connections of this node</returns>
    public abstract List<Connection> GetOutConnections(int state);
}
