#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public Vector2 InPos;
    public Vector2 OutPos;

    public Action OnClickButton;

    public Connection(Vector2 inPos, Vector2 outPos, Action onClickButton = null)
    {
        InPos = inPos;
        OutPos = outPos;
        OnClickButton = onClickButton;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            InPos,
            OutPos,
            InPos + Vector2.left * 50f,
            OutPos - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (OnClickButton != null)
            Handles.Button((InPos + OutPos) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap);
    }

    public void ProcessEvent(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if (OnClickButton != null)
                {
                    if (new Rect((InPos + OutPos) * 0.5f - new Vector2(4,4), new Vector2(8,8)).Contains(e.mousePosition))
                    {
                        e.Use();
                        OnClickButton();
                    }
                }
                break;
        }
    }
}

#endif