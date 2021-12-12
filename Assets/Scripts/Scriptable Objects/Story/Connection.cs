using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    // References to connection points
    public Vector2 InPos;
    public Vector2 OutPos;

    // Reference to remove method
    public Action OnClickRemoveConnection;

    // Create a new Connection
    public Connection(Vector2 inPos, Vector2 outPos, Action onClickRemoveConnection = null)
    {
        // Set connection Pos references
        InPos = inPos;
        OutPos = outPos;
        OnClickRemoveConnection = onClickRemoveConnection;
    }

    // This is called whenever the screen gets redrawn
    public void Draw()
    {
        // Draw a smooth curve between the two Poss
        Handles.DrawBezier(
            InPos,
            OutPos,
            InPos + Vector2.left * 50f,
            OutPos - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        // Draw a Button in the middle of the curve that removes the connection
        Handles.Button((InPos + OutPos) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap);
    }

    public void ProcessEvent(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if (new Rect((InPos + OutPos) * 0.5f - new Vector2(4,4), new Vector2(8,8)).Contains(e.mousePosition))
                {
                    e.Use();
                    if (OnClickRemoveConnection != null)
                    {
                        OnClickRemoveConnection();
                    }
                }
                break;
        }
    }
}