using System;
using System.Linq;
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
    [SerializeField] private bool _isReceivingState = false;
    public bool IsReceivingState
    {
        get => _isReceivingState;
#if UNITY_EDITOR
        set
        {
            if (value == true)
            {
                if (ParentQuest.DialogueNodes.Exists( e => e != this && e is DialogueLine && ((DialogueLine)e).IsReceivingState))
                {
                    throw new Exception("There already is a receiving state in this lines parent quest");
                }
                _isReceivingState = true;
                HasAnswers = false;
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
                selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
            }
            else
            {
                _isReceivingState = false;
                style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
                selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            }
            GUI.changed = true;
        }
#endif
    }
    public DialogueNode NextLeft;
    public DialogueNode NextRight;
    public override DialogueLine NextLine { get { return this; } }

#if UNITY_EDITOR

    public ConnectionPoint OutPointRight, OutPointLeft;

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

        if (NextLeft != null && !ParentQuest.DialogueNodes.Contains(NextLeft)) ParentQuest.DialogueNodes.Add(NextLeft);
        if (NextRight != null && !ParentQuest.DialogueNodes.Contains(NextRight)) ParentQuest.DialogueNodes.Add(NextRight);

        IsReceivingState = IsReceivingState; // set this to itself so that styles are set. it's stupid but it's quick.
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

        if (Notes != String.Empty && Notes != null) DrawNotification(0);
        if (HasAnswers && NextLeft == null) DrawNotification(1);
        if (Text == null || Text == String.Empty) DrawNotification(2);
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

#endif

}
