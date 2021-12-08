using System;
using UnityEditor;
using UnityEngine;
public class Connection
{
    // References to connection points
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    // Reference to remove method
    public Action<Connection> OnClickRemoveConnection;

    // Create a new Connection
    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        // Set connection point references
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        // Set remove method reference
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public static Connection SetNewConnection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        // get in and out node
        Node inNode = inPoint.node;
        Node outNode = outPoint.node;

        // Behaviour: DialogueLine -> DialogueLine
        if ((outNode is DialogueLineNode) && (inNode is DialogueLineNode))
        {
            DialogueLineNode lineNode = (DialogueLineNode)outNode;
            DialogueLine line = (inNode as DialogueLineNode).Line;

            // Select whether to put the line in nextLeft or nextRight
            if (outPoint == lineNode.outPointRight)
            {
                lineNode.Line.NextRight = line;
            }
            else if (outPoint == lineNode.outPointLeft)
            {
                lineNode.Line.NextRight = line;
            }
            
            // Mark the out Dialogue Line for saving 
            EditorUtility.SetDirty(lineNode.Line);
        }

        // Behaviour: Quest -> DialogueLine
        if ((outNode is QuestNode) && (inNode is DialogueLineNode))
        {
            QuestNode questNode = (QuestNode)outNode;
            DialogueLine line = (inNode as DialogueLineNode).Line;

            // Select whether to put the line in nextLeft or nextRight
            if (outPoint == questNode.outPointPreceding )
            {
                questNode.Quest.PrecedingStartLine = line;
            }
            else if (outPoint == questNode.outPointSucceding)
            {
                questNode.Quest.SucceedingStartLine = line;
            }

            // Mark the quest for saving
            EditorUtility.SetDirty(questNode.Quest);
        }

        // Beahviour: Quest -> Quest
        if ((outNode is QuestNode) && (inNode is QuestNode))
        {
            QuestNode questNode = outNode as QuestNode;

            // Get the index of the selected Connection Point of the quest Node 
            int index = questNode.outPoints.IndexOf(outPoint);

            // Set the link at the index to point to the correct next quest
            Quest.Link link = questNode.Quest.Links[index];
            link.NextQuest = (inNode as QuestNode).Quest;
            questNode.Quest.Links[index] = link;

            // Mark the quest for saving
            EditorUtility.SetDirty(questNode.Quest);
        }

        // Save asset changes
        AssetDatabase.SaveAssets();

        return new Connection(inPoint, outPoint, OnClickRemoveConnection);
    }

    // This is called whenever the screen gets redrawn
    public void Draw()
    {

        // If the outPoint is the left side of a dialogueLineNode which has the left side disabled, don't draw the connection;
        if (outPoint.node is DialogueLineNode && outPoint == (outPoint.node as DialogueLineNode).outPointLeft && !(outPoint.node as DialogueLineNode).Line.HasAnswers) return;

        // Draw a smooth curve between the two points
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        // Draw a Button in the middle of the curve that removes the connection
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
    public void Remove()
    {
        // Get references to the in and out nodes
        Node outNode = outPoint.node;
        Node inNode = inPoint.node;

        // Behaviour for a connection between two dialogue lines
        if (outNode is DialogueLineNode)
        {
            // Find out if the connection is on the left or right output and set the relevant reference in the DialogueLine to null
            DialogueLineNode lineNode = (DialogueLineNode)outNode;
            if (outPoint == lineNode.outPointLeft)
            {
                lineNode.Line.NextLeft = null;
            }
            if (outPoint == lineNode.outPointRight)
            {
                lineNode.Line.NextRight = null;
            }

            // Mark the edited line for saving
            EditorUtility.SetDirty(lineNode.Line);
        }

        // Behaviour for a connection between a questNode and a DialogueLIne
        if ((outNode is QuestNode) && (inNode is DialogueLineNode))
        {
            QuestNode questNode = (QuestNode)outNode;
            // Find out if the connection is on the preceding or succeding output and set the relevant reference in the Quest to null
            if (outPoint == questNode.outPointPreceding)
            {
                questNode.Quest.PrecedingStartLine = null;
            }
            if (outPoint == questNode.outPointSucceding)
            {
                questNode.Quest.SucceedingStartLine = null;
            }
            
            // Mark the edited quest for saving
            EditorUtility.SetDirty(questNode.Quest);
        }

        // Behaviour for a connection between two quests
        if ((outNode is QuestNode) && (inNode is QuestNode))
        {
            // Get a reference to the outNode
            Quest quest = (outNode as QuestNode).Quest;
            // Determine the index of the connection point the connection is coming from
            int index = (outNode as QuestNode).outPoints.IndexOf(outPoint);
            
            if (index > 0 && index < quest.Links.Count)
            {
                // Set the link at the index to point to null
                Quest.Link link = quest.Links[index];
                link.NextQuest = null;
                quest.Links[index] = link;  
            }
            
            // Mark the edited quest for saving
            EditorUtility.SetDirty(quest);          
        }

        // Save changes to assets
        AssetDatabase.SaveAssets();

        // remove connectionpoints references to connection;
        inPoint.connection = null;
        outPoint.connection = null;
    }
}