using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DialogueLineNode : Node
{
    public DialogueLine Line;

    public ConnectionPoint outPointLeft;
    public ConnectionPoint outPointRight
    {
        get { return this.outPoint; }
        set { this.outPoint = value; }
    }
    // Constructor mostly does the same as base node but with some extra stuff
    public DialogueLineNode(DialogueLine line, Vector2 position, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
    :base(position, 100, 50, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
        // Save line ref
        Line = line;
        outPointLeft = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        outPointLeft.YPercent = 0.66f;
    }

    public override void SelectAssociatedObject()
    {
        Selection.activeObject = Line;
    }

    public override void Draw()
    {
        if (Line.HasAnswers)
        {
            outPoint.YPercent = 0.33f;
            inPoint.YPercent = 0.33f;
            rect.height = 75;

            outPointLeft.Draw();
        }
        else
        {
            outPoint.YPercent = 0.5f;
            inPoint.YPercent = 0.5f;
            rect.height = 50;
        }
        base.Draw();
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Line.name, style);
    }
}