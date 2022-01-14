using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DialogueLine : DialogueNode
{
    public string Text;
    public string Notes;
    public string AnswerLeft;
    public string AnswerRight;

    public bool HasAnswers = false;

    public DialogueNode NextLeft;
    public DialogueNode NextRight;

    public ConnectionPoint OutPointRight, OutPointLeft;

    public override DialogueLine NextLine { get { return this; } }

    /// <summary>
    /// Return a list of outConnections from this to 
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public override List<Connection> GetOutConnections(int state = 0)
    {
        List<Connection> connections = new List<Connection>();

        if (NextRight != null && ParentQuest.DialogueNodes.Contains(NextRight))
        {
            connections.Add(new Connection(NextRight.InPoint.Center, OutPointRight.Center, () => OnConnectionClick(0)));
        }

        if (HasAnswers)
        {
            if (NextLeft != null && ParentQuest.DialogueNodes.Contains(NextRight))
            {
                connections.Add(new Connection(NextLeft.InPoint.Center, OutPointLeft.Center, () => OnConnectionClick(1)));
            }
        }

        return connections;
    }

    /// <summary>
    /// Initialize Dat when this Node is enabled
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();

        OutPointRight = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick, 0);
        OutPointLeft = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick, 1);
    }

    public override void OnOutPointClick(int index)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null)
        {
            // ---- CONNECT TO DIALOGUE LINE ----
            if (inPoint.Parent is DialogueNode)
            {
                switch (index)
                {
                    // Connect to Right
                    case 0:
                        NextRight = (inPoint.Parent as DialogueNode);
                        break;
                    // Connect to Left
                    case 1:
                        NextLeft = (inPoint.Parent as DialogueNode);
                        break;
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
        {
            ConnectionPoint.selectedOutPoint = index == 1 ? OutPointLeft : OutPointRight;
        }
    }

    /// <summary>
    /// Action passed to an Connection for OnClick Handling
    /// </summary>
    /// <param name="index">0 for right, 1 for left</param>
    private void OnConnectionClick(int index)
    {
        switch(index)
        {
            //  ---- RIGHT -----
            case 0:
                NextRight = null;
                break;

            // ---- LEFT ----
            case 1:
                NextLeft = null;
                break;
        }
    }

    // ---- OVERRIDES ----  
    public override void Draw(Vector2 offset, int state = 0)
    {
        InPoint.Draw();
        OutPointRight.Draw();
        if (HasAnswers) OutPointLeft.Draw();
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        Size.y = HasAnswers ? 65 : 40;

        base.Draw(offset, state);
        GUI.Label(rect, Title, LabelStyle);
    }
    public override void ProcessEvent(Event e, int state = 0, List<StoryNode> relatedNodes = null)
    {
        InPoint.ProcessEvent(e);
        OutPointRight.ProcessEvent(e);
        if (HasAnswers) OutPointLeft.ProcessEvent(e);
        base.ProcessEvent(e, state, relatedNodes);
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && rect.Contains(e.mousePosition))
                {
                    // Connect pending OutPoint
                    if (ConnectionPoint.selectedOutPoint != null)
                    {
                        OnInPointClick();
                    }
                    e.Use();
                }
                break;
        }
    }
}
