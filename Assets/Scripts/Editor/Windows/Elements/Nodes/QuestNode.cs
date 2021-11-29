using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestNode : Node
{
    // Reference to the nodes quest
    public Quest Quest;

    // Actions for showing this quests dialogue and for OnDragEnd
    private Action<QuestNode> OnClickShowDialogue;
    private Action<QuestNode> DragEnd;

    // outpoints for preceding and succeding lines
    public ConnectionPoint outPointPreceding { get { return outPoints[0]; } }
    public ConnectionPoint outPointSucceding { get { return outPoints[1]; } }

    // Constructor
    public QuestNode (Quest quest, NodeData nodeData, Action<QuestNode> onClickShowDialogue, Action<QuestNode> dragEnd)
    :base(quest.EditorPos, 100, nodeData)
    {
        // Set variables
        Quest = quest;
        OnClickShowDialogue = onClickShowDialogue;
        DragEnd = dragEnd;
        SetOutNodeCount(Quest.Links.Count);
    }

    /// <summary>
    /// Gets called when this nodes associated object is supposed to be selected
    /// </summary>
    public override void OnSelection()
    {
        Selection.activeObject = Quest;
    }
    /// <summary>
    /// Gets called when the drag event ends
    /// </summary>
    public override void OnDragEnd()
    {
        if (DragEnd != null)
        {
            DragEnd(this);
        }
    }

    /// <summary>
    /// Gets called on - you guessed it - doubleclicks 
    /// </summary>
    public override void OnDoubleclick()
    {
        OnClickShowDialogue(this);
    }

    /// <summary>
    /// Gets called every time the window is repainted
    /// </summary>
    public override void Draw()
    {
        base.Draw();

        // Create a label with the boxes title
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Quest.Title, labelStyle);

        // Increase Size to fit title, if neccessary
        Vector2 size = labelStyle.CalcSize( new GUIContent( Quest.Title ) );
        rect.width = Mathf.Max(rect.width, size.x + 20);

        // Draw Node Labels
        labelStyle.padding.top += 5;
        labelStyle.padding.right += 10;
        labelStyle.alignment = TextAnchor.UpperRight;
        switch (DialogueEditor.Instance.State)
        {
            case DialogueEditor.WindowState.DialogueView:
                // In DialogueView there are two labels and a fixed width
                labelStyle.padding.top += 25;
                GUI.Label(rect, "Preceeding Dialogue", labelStyle);
                labelStyle.padding.top += 25;
                GUI.Label(rect, "Succeeding Dialogue", labelStyle);

                rect.width = 150;
                break;

            case DialogueEditor.WindowState.QuestView:
                // In QuestView, there is a flexible amount of nodes and width
                foreach (Quest.Link link in Quest.Links)
                {
                    labelStyle.padding.top += 25;
                    GUI.Label(rect, link.Potion.Name, labelStyle);

                    // Increase label width if neccessary
                    rect.width = 100;
                    size = labelStyle.CalcSize( new GUIContent( link.Potion.Name ) );
                    rect.width = Mathf.Max(rect.width, size.x + 20);
                }
                break;
        }
        
    }

    /// <summary>
    /// populates the context menu with options
    /// </summary>
    /// <param name="menu">Reference to the Menu</param>
    public override void FillContextMenu(GenericMenu menu)
    {
        if (DialogueEditor.Instance.State == DialogueEditor.WindowState.QuestView)
        {
            menu.AddItem(new GUIContent("Open Dialogue"), false, () => { OnClickShowDialogue(this); });
        }
        base.FillContextMenu(menu);
    }
}