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

    private float dialogueViewWidth, questViewWidth;

    // Constructor
    public QuestNode (Quest quest, NodeData nodeData, Action<QuestNode> onClickShowDialogue, Action<QuestNode> dragEnd)
    :base(quest.EditorPos, 100, nodeData)
    {
        // Set variables
        Quest = quest;
        OnClickShowDialogue = onClickShowDialogue;
        DragEnd = dragEnd;
        UpdateContent();
    }

    public void UpdateContent()
    {
        Debug.Log(Quest.Links.Count);
        SetOutPointCount(Mathf.Max(Quest.Links.Count, 2));
        displayedOutPoints = Quest.Links.Count;

        // Set size to fit title
        float titleWidth = new GUIStyle().CalcSize(new GUIContent(Quest.Title)).x;
        dialogueViewWidth = Mathf.Max(150, titleWidth + 20);
        questViewWidth = Mathf.Max(100, titleWidth + 20);

        foreach (Quest.Link link in Quest.Links)
        {
            // Increase width if neccessary
            float potionNameWidth = new GUIStyle().CalcSize(new GUIContent(link.Potion.Name)).x;
            questViewWidth = Mathf.Max(questViewWidth, potionNameWidth + 20);
        }
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

        // Draw Node Labels
        labelStyle.padding.top += 5;
        labelStyle.padding.right += 10;
        labelStyle.alignment = TextAnchor.UpperRight;
        switch (DialogueEditor.Instance.State)
        {
            case DialogueEditor.WindowState.DialogueView:
                // In DialogueView there are two labels and a fixed width
                rect.width = dialogueViewWidth;
                labelStyle.padding.top += 25;
                GUI.Label(rect, "Preceeding Dialogue", labelStyle);
                labelStyle.padding.top += 25;
                GUI.Label(rect, "Succeeding Dialogue", labelStyle);
                break;

            case DialogueEditor.WindowState.QuestView:
                // In QuestView, there is a flexible amount of nodes and width
                rect.width = questViewWidth;
                foreach (Quest.Link link in Quest.Links)
                {
                    // set label
                    labelStyle.padding.top += 25;
                    GUI.Label(rect, link.Potion.Name, labelStyle);
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