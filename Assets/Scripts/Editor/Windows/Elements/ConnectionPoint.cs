using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define types of connection points
public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    // Rectangle reference
    public Rect rect;

    // Is this in or out?
    public ConnectionPointType type;

    // Reference to parent node
    public Node node;

    public Connection connection;

    // style reference
    public GUIStyle style;

    // Reference to the Connectionpoint click ethod
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public float YPercent = 0.5f;

    // Create a new connection point
    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        // Set references
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;

        // Create rectangle
        rect = new Rect(0, 0, 10f, 20f);
    }

    // Draw this object
    public void Draw()
    {
        // set rect y in relation to the node
        rect.y = node.rect.y + (node.rect.height * YPercent) - rect.height * 0.5f;

        // Set rec x in relation to the node and to connection type
        switch (type)
        {
            // left
            case ConnectionPointType.In:
            rect.x = node.rect.x - rect.width + 8f;
            break;

            // out
            case ConnectionPointType.Out:
            rect.x = node.rect.x + node.rect.width - 8f;
            break;
            
        }

        // if rectangle is clicked, call click method
        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}