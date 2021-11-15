using System;
using UnityEditor;
using UnityEngine;
public class Connection
{
    // References to connection points
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    // Reference to remove method
    public Action<Connection> OnClickRemoveConnection;

    // Create a new Connection
    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        // Set connection point references
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        // Set remove method reference
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    // This is called whenever the screen gets redrawn
    public void Draw()
    {
        // Draw a smooth curve between the two points
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        // If there is a button on the connection, call the remove method
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}