using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct NodeData
{
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

    // References for connection points
    public ConnectionPoint inPoint;
    public List<ConnectionPoint> outPoints;

    // Styles
    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle style;

    // On Remove method
    public Action<ConnectionPoint> OnClickInPoint;
    public Action<ConnectionPoint> OnClickOutPoint;
    public Action<Node> OnClickRemoveNode;

    // Saves the last time this was clicked to detect doubleclicks
    private double lastClicked;
    private float currentHeight { get { return 50 + displayedOutPoints * 25; } }
    public float displayedOutPoints = 0;

    // Create a new Node
    public Node(Vector2 position, float width, NodeData nodeData)
    {
        // Set all the styles
        defaultNodeStyle = new GUIStyle();
        defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        style = defaultNodeStyle;

        // Set Actions
        OnClickRemoveNode = nodeData.OnClickRemoveNode;
        OnClickOutPoint = nodeData.OnClickOutPoint;
        OnClickInPoint = nodeData.OnClickInPoint;

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, 50);
        outPoints = new List<ConnectionPoint>();

        rect = new Rect(position.x, position.y, width, 0);
    }

    // Set the number of out nodes
    public void SetOutPointCount (int count)
    {
        if (count > outPoints.Count)
        {
            for (int i = outPoints.Count; i < count; i++)
            {
                ConnectionPoint outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, currentHeight);
                outPoints.Add(outPoint);
                displayedOutPoints++;
            }
        }
        else
        {
            outPoints.GetRange(count, outPoints.Count - count).ForEach(e =>
            {
                Connection connection = (e as ConnectionPoint).connection;
                connection?.OnClickRemoveConnection(connection);
            });
            displayedOutPoints = count;
            outPoints.RemoveRange(count, outPoints.Count - count);
        }
    }

    // Move the Node to a position;
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    // Draw window
    public virtual void Draw()
    {
        rect.height = currentHeight;
        // Draw ConnectionPoints
        inPoint.Draw();
        for (int i = 0; i < Mathf.Min(displayedOutPoints, outPoints.Count); i++)
        {
            outPoints[i].Draw();
        }
        // Draw own box
        GUI.Box(rect, "", style);
    }


    // On window events
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                // on leftclick, make node selected and dragged
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true; // queue redraw
                        isSelected = true;
                        style = selectedNodeStyle;

                        OnSelection();

                        // Detect Doubleclicks
                        if (EditorApplication.timeSinceStartup - lastClicked < .25)
                        {
                            OnDoubleclick();
                        }
                        lastClicked = EditorApplication.timeSinceStartup;
                    }
                    else
                    {
                        GUI.changed = true; // queue redraw
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                // on node rightclicks
                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    FillContextMenu(genericMenu);
                    genericMenu.ShowAsContext();
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

    // Overidables
    public virtual void OnDragEnd() { }
    public virtual void OnDoubleclick() { }
    public virtual void OnSelection() { }

    // Create a context menu
    public virtual void FillContextMenu(GenericMenu menu)
    {
        // Just a remove button
        menu.AddItem(new GUIContent("Remove node"), false, () => { OnClickRemoveNode(this); });
    }
}