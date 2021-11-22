using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestNode : Node
{
    public Quest Quest;
    private Action<QuestNode> ShowDialogue;
    private Action<QuestNode> DragEnd;

    public List<ConnectionPoint> linkOutPoints;

    public ConnectionPoint outPointSucceding;
    public ConnectionPoint outPointPreceding
    {
        get { return base.outPoint; }
    }

    public QuestNode (Quest quest, NodeData nodeData, Action<QuestNode> showDialogue, Action<QuestNode> dragEnd)
    :base(quest.EditorPos, 100, 50, nodeData)
    {
        Quest = quest;
        ShowDialogue = showDialogue;
        DragEnd = dragEnd;
        linkOutPoints = new List<ConnectionPoint>();

        foreach (Quest.Link link in Quest.Links)
        {
            linkOutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, 25 * (linkOutPoints.Count + 1)));
        }
    }

    public override void SelectAssociatedObject()
    {
        Selection.activeObject = Quest;
    }
    public override void OnDragEnd()
    {
        if (DragEnd != null)
        {
            DragEnd(this);
        }
    }

    public override void Draw()
    {
        switch (DialogueEditor.Instance.State)
        {
            case DialogueEditor.WindowState.DialogueView:
                rect.height = 50;
                outPoint.Draw();
                break;

            case DialogueEditor.WindowState.QuestView:
                rect.height = 25 * (Quest.Links.Count + 1);
                inPoint.Draw();
                foreach (ConnectionPoint point in linkOutPoints)
                {
                    point.Draw();
                }
                break;
        }
        // Draw own box
        GUI.Box(rect, "", style);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Quest.Title, labelStyle);
    }

    public override void FillContextMenu(GenericMenu menu)
    {
        if (DialogueEditor.Instance.State == DialogueEditor.WindowState.QuestView)
        {
            menu.AddItem(new GUIContent("Open Dialogue"), false, OnClickShowDialogue);
        }
        base.FillContextMenu(menu);
    }

    private void OnClickShowDialogue()
    {
        if (ShowDialogue != null)
        {
            ShowDialogue(this);
        }
    }
}