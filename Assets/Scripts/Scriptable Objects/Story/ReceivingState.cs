using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReceivingState : DialogueNode
{
    private DialogueNode NextNode;
    public override DialogueLine NextLine => NextNode.NextLine;
    public ConnectionPoint OutPoint;

    public override void OnEnable()
    {
        base.OnEnable();
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick);
    }

    public override void Draw(Vector2 offset, int state = 0)
    {
        Size.y = 40;
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        InPoint.Draw();
        OutPoint.Draw();
        base.Draw(offset, state);
        GUI.Label(rect, Title, LabelStyle);
    }

    public override void ProcessEvent(Event e, int state = 0)
    {
        InPoint.ProcessEvent(e);
        OutPoint.ProcessEvent(e);
        base.ProcessEvent(e, state);
    }

    public override List<Connection> GetOutConnections(int state)
    {   
        List<Connection> list = new List<Connection>();
        if (NextNode != null)
            list.Add(new Connection(NextNode.InPoint.Center, OutPoint.Center, () => NextNode = null));
        return list;
    }

    public override void OnOutPointClick(int _)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null)
        {
            // Create a Connection to another node

            // ---- CONNECTION TO QUEST NODE ----
            if (inPoint.Parent is DialogueNode)
            {
                NextNode = (DialogueLine)inPoint.Parent;
            }

            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
        {
            ConnectionPoint.selectedOutPoint = OutPoint;
        }
    }
}