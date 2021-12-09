using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    // References to connection points
    public Vector2 InPos;
    public Vector2 OutPos;

    // Reference to remove method
    public Action<Connection> OnClickRemoveConnection;

    // Create a new Connection
    public Connection(Vector2 inPos, Vector2 outPos)
    {
        // Set connection Pos references
        InPos = inPos;
        OutPos = outPos;
    }

    // This is called whenever the screen gets redrawn
    public void Draw()
    {
        Vector2 inPos = InPos;
        Vector2 outPos = OutPos;
        // Draw a smooth curve between the two Poss
        Handles.DrawBezier(
            inPos,
            outPos,
            inPos + Vector2.left * 50f,
            outPos - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        // Draw a Button in the middle of the curve that removes the connection
        if (Handles.Button((inPos + outPos) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {

        }
    }
}