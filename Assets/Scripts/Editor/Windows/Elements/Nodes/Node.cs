using System;
using UnityEditor;
using UnityEngine;

public struct NodeData
{
    public GUIStyle nodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;
    public Action<ConnectionPoint> OnClickInPoint;
    public Action<ConnectionPoint> OnClickOutPoint;
    public Action<Node> OnClickRemoveNode;
}
public class Node
{

    // Reference to rect
    public Rect rect;

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
    public Node(Vector2 position, float width, float height, NodeData nodeData)
    {
        // Set references
        rect = new Rect(position.x, position.y, width, height);
        style = nodeData.nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, nodeData.inPointStyle, nodeData.OnClickInPoint, height / 2);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, height / 2);
        defaultNodeStyle = nodeData.nodeStyle;
        selectedNodeStyle = nodeData.selectedNodeStyle;
        OnRemoveNode = nodeData.OnClickRemoveNode;
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
        GUI.Box(rect, "", style);
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
                if (isDragged)
                {
                    isDragged = false;
                    OnDragEnd();
                }
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

    public virtual void OnDragEnd() { } // just for overrides

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