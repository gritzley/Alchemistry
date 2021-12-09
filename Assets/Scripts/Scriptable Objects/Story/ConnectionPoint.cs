using UnityEngine;
using UnityEditor;
public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Vector2 Center { get { return rect.center; } } 
    ConnectionPointType type;
    GUIStyle style;
    StoryNode parent;
    Rect rect;
    int index;
    public ConnectionPoint(StoryNode parent, ConnectionPointType type, int index = 0)
    {
        this.parent = parent;
        this.type = type;
        this.index = index;
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

    public void Draw()
    {
        float xPos = type == ConnectionPointType.In ? parent.rect.xMin : parent.rect .xMax - 10;
        float yPos = parent.rect.yMin + 10 + index * 25;
        rect.position = new Vector2(xPos, yPos);
        if (GUI.Button(rect, "", style))
        {

        }
    }
}