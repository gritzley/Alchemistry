using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Node
{
    // Reference to rect
    public Rect rect;

    // empty title is nices than using "" for everything
    public string title;

    // Flags
    public bool isDragged;
    public bool isSelected;

    // Refs for connection points
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    // Styles
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle; 

    // On Remove method
    public Action<Node> OnRemoveNode;

    // Create a new Node
    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
    {
        // Set references
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
    }

    // Move the Node to a position;
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    // Draw window
    public virtual void Draw()
    {
        // Draw children
        inPoint.Draw();
        outPoint.Draw();

        // Draw own box
        GUI.Box(rect, title, style);
    }

    public virtual void SelectAssociatedObject()
    {
        // override this
    }

    // On window events
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {

            case EventType.MouseDown:
                if (e.button == 0)
                {
                    // on leftclick, make node selected and dragged
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true; // queue redraw
                        isSelected = true;
                        style = selectedNodeStyle;

                        SelectAssociatedObject();
                    }
                    else
                    {
                        GUI.changed = true; // queue redraw
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                // on node rightclicks
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            // on mouseup stop dragging
            case EventType.MouseUp:
                isDragged = false;
                break;

            // in mousemove with leftclick, move this object
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    // Create a context menu
    private void ProcessContextMenu()
    {
        // Just a remove button
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    // Remove node method
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}