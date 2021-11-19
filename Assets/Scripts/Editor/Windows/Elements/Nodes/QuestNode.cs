using System;
using UnityEditor;
using UnityEngine;

public class QuestNode : Node
{
    public Quest Quest;
    public QuestNode (Quest quest, NodeData nodeData)
    :base(quest.EditorPos, 100, 50, nodeData)
    {
        Quest = quest;
    }

    public override void SelectAssociatedObject()
    {
        Selection.activeObject = Quest;
    }
    public override void OnDragEnd()
    {
        Quest.EditorPos = rect.position;
    }

    public override void Draw()
    {

        base.Draw();
    }
}