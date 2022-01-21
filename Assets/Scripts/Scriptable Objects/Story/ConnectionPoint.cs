#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public static ConnectionPoint selectedInPoint, selectedOutPoint;
    public Vector2 Center { get { return rect.center; } }
    public StoryNode Parent { get { return parent; } }
    public int Index { get { return index; } }
    StoryNode parent;
    ConnectionPointType type;
    GUIStyle style;
    Rect rect;
    int index;
    Action<int> OnClick;
    public ConnectionPoint(StoryNode parent, ConnectionPointType type, Action<int> OnClick, int index = 0)
    {
        this.parent = parent;
        this.type = type;
        this.index = index;
        this.OnClick = OnClick;
        style = new GUIStyle();
        switch (type)
        {
            case ConnectionPointType.In:
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
                style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
                style.border = new RectOffset(4, 4, 12, 12);
                break;

            case ConnectionPointType.Out:
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
                style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
                style.border = new RectOffset(4, 4, 12, 12);
                break;
        }
        rect = new Rect(0, 0, 10, 20);
    }

    public void ProcessEvent(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(e.mousePosition))
                {
                    OnClick(index);
                    e.Use();
                }
                break;
        }
        
    }

    public void Draw()
    {
        float xPos = type == ConnectionPointType.In ? parent.rect.xMin - 2 : parent.rect .xMax - 8;
        float yPos = parent.rect.yMin + 10 + index * 25;
        rect.position = new Vector2(xPos, yPos);

        GUI.Button(rect, "", style);
    }
}

#endif