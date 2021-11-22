using System;
using UnityEditor;
using UnityEngine;

public class DialogueLineNode : Node
{
    // Reference to Dialogue Line
    public DialogueLine Line;

    // Reference to parent node
    private QuestNode parent;

    // Create a second out point for the left side answer
    public ConnectionPoint outPointLeft;
    // Create a reference for the right side answer that really just points to the normal out Point
    public ConnectionPoint outPointRight
    {
        get { return base.outPoint; }
    }
    /// <summary>
    /// Create a new DialogueLineNode from a line and a set of node data
    /// </summary>
    /// <param name="line">The associated DialogueLine</param>
    /// <param name="nodeData">A NodeData Object to use for the node</param>
    public DialogueLineNode(QuestNode parent, DialogueLine line, NodeData nodeData)
    :base(line.EditorPos + parent.rect.position, 100, 50, nodeData)
    {
        // Save line ref
        Line = line;
        // Save parent ref
        this.parent = parent;
        // Create the second out point
        outPointLeft = new ConnectionPoint(this, ConnectionPointType.Out, nodeData.outPointStyle, nodeData.OnClickOutPoint, 50);
    }

    /// <summary>
    /// On select, set the inspector selection to the associated DialogueLine
    /// </summary>
    public override void SelectAssociatedObject()
    {
        Selection.activeObject = Line;
    }

    /// <summary>
    /// OnDragEnd, override the Lines EditorPosition
    /// </summary>
    public override void OnDragEnd()
    {
        // Set this 
        Line.EditorPos = rect.position - parent.rect.position;

        // mark Line for savig
        EditorUtility.SetDirty(Line);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Override the Draw Method of the Base Class
    /// </summary>
    public override void Draw()
    {
        // Adjust the look of the node depending on wheter it has answers
        if (Line.HasAnswers)
        {
            rect.height = 75;

            outPointLeft.Draw();
        }
        else
        {
            rect.height = 50;
        }
        // draw the base
        base.Draw();
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Line.Title, labelStyle);
    }
}
