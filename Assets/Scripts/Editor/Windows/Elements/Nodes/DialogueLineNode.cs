using System;
using UnityEditor;
using UnityEngine;

public class DialogueLineNode : Node
{
    // Reference to Dialogue Line
    public DialogueLine Line;

    // Reference to parent node
    private QuestNode parent;

    // easier refrences to the out points
    public ConnectionPoint outPointLeft { get { return base.outPoints[1]; } }
    public ConnectionPoint outPointRight { get { return base.outPoints[0]; } }

    /// <summary>
    /// Create a new DialogueLineNode from a line and a set of node data
    /// </summary>
    /// <param name="line">The associated DialogueLine</param>
    /// <param name="nodeData">A NodeData Object to use for the node</param>
    public DialogueLineNode(QuestNode parent, DialogueLine line, NodeData nodeData)
    :base(line.EditorPos + parent.rect.position, 100, nodeData)
    {
        Line = line;
        this.parent = parent;
        SetOutNodeCount(2);
    }

    /// <summary>
    /// On select, set the inspector selection to the associated DialogueLine
    /// </summary>
    public override void OnSelection()
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
        SetOutNodeCount(Line.HasAnswers ? 2 : 1);
        // draw the base
        base.Draw();

        // Draw Node Content
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.padding = new RectOffset(0, 0, 10, 0);
        GUI.Label(rect, Line.Title, labelStyle);

        labelStyle.padding.top += 5;
        labelStyle.padding.right += 10;
        labelStyle.alignment = TextAnchor.UpperRight;
        if (Line.HasAnswers) {
            labelStyle.padding.top += 25;
            GUI.Label(rect, "Right Answer", labelStyle);
            labelStyle.padding.top += 25;
            GUI.Label(rect, "Left Answer", labelStyle);
        }
    }
}
