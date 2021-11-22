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
    private Action<QuestNode> ShowDialogue;
    private Action<QuestNode> DragEnd;

    // A list of connectionPoints for the links
    public List<ConnectionPoint> linkOutPoints;

    // outpoints for preceding and succeding lines
    public ConnectionPoint outPointSucceding;
    public ConnectionPoint outPointPreceding;

    // Constructor
    public QuestNode (Quest quest, NodeData nodeData, Action<QuestNode> showDialogue, Action<QuestNode> dragEnd)
    :base(quest.EditorPos, 100, 75, nodeData)
    {
        // Set variables
        Quest = quest;
        ShowDialogue = showDialogue;
        DragEnd = dragEnd;

        // Fill linkOutPoints with links
        linkOutPoints = new List<ConnectionPoint>();
        foreach (Quest.Link link in Quest.Links)
        {
            linkOutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, 25 * (linkOutPoints.Count + 1)));
        }

        // Set the preceding and succeding out points
        outPointPreceding = new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, 25);
        outPointSucceding = new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, 50);
    }

    /// <summary>
    /// Gets called when this nodes associated object is supposed to be selected
    /// </summary>
    public override void SelectAssociatedObject()
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
    /// Gets called every time the window is repainted
    /// </summary>
    public override void Draw()
    {
        // Draw dirrefrently depending on editor view state
        switch (DialogueEditor.Instance.State)
        {
            case DialogueEditor.WindowState.DialogueView:
                // In DialogueView, this has a fixed height and number of outputs
                // Also there is no in point
                rect.height = 75;
                outPointPreceding.Draw();
                outPointSucceding.Draw();
                break;

            case DialogueEditor.WindowState.QuestView:
                // In QuestView, the height and number of outpoints depends on the number of links
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

        // Create a label with the boxes title
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Quest.Title, labelStyle);
    }

    /// <summary>
    /// populates the context menu with options
    /// </summary>
    /// <param name="menu">Reference to the Menu</param>
    public override void FillContextMenu(GenericMenu menu)
    {
        if (DialogueEditor.Instance.State == DialogueEditor.WindowState.QuestView)
        {
            menu.AddItem(new GUIContent("Open Dialogue"), false, OnClickShowDialogue);
        }
        base.FillContextMenu(menu);
    }

    /// <summary>
    /// Gets called when clicking on the show dialogue option in the context menu
    /// </summary>
    private void OnClickShowDialogue()
    {
        if (ShowDialogue != null)
        {
            ShowDialogue(this);
        }
    }
}