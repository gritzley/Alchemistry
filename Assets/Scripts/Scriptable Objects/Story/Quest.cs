using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quest : StoryNode
{

    // ---- STORY TREE ----
    public DialogueNode PrecedingStartNode;
    public DialogueLine CurrentLine;
    public CustomerDefinition Customer;

    // ---- LINKS ----
    [System.Serializable]
    public struct Link
    {
        public PotionDefinition Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    // ---- EDITOR ----
    private ConnectionPoint OutPoint;
    public List<DialogueNode> DialogueNodes;
    public Action<Quest> ViewDialogue;
    
    // CONSTRUCTOR
    public Quest() : base()
    {
        Links = new List<Link>();
        DialogueNodes = new List<DialogueNode>();
    }

    /// <summary>
    /// Set initial Values when enabling this quest
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateLinks();
        CurrentLine = PrecedingStartNode?.NextLine;
        Size.y = 40;
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick);
        DialogueNodes = DialogueNodes.Where(e => e != null).ToList(); // thus the grand culling began
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        List<PotionDefinition> potions = PotionDefinition.GetAllPotionAssets();

        Links.RemoveAll( e => !potions.Contains(e.Potion));

        potions
        .Except( Links.Select(e => e.Potion).Where(e => potions.Contains(e)) )
        .ToList()
        .ForEach( e => Links.Add(new Link() { Potion = e }));

        Links = Links.OrderBy(e => e.Potion.name).ToList();
    }

    public bool AdvanceDialogue(int i = 0)
    {
        switch (i)
        {
            case 0:
                if (CurrentLine.NextRight != null)
                {
                    CurrentLine = CurrentLine.NextRight.NextLine;
                    return true;
                }
                break;
            case 1:
                if (CurrentLine.HasAnswers && CurrentLine.NextLeft != null)
                {
                    CurrentLine = CurrentLine.NextLeft.NextLine;
                    return true;
                }
                break;
        }
        return false;
    }

    public Quest GetNextQuest(PotionDefinition potion)
    {
        foreach (Link link in Links)
            if (link.Potion == potion)
                return link.NextQuest;

        Debug.LogWarning($"Quest {name} has no link for potion {potion.name}.");
        return null;
    }

    public override void OnOutPointClick(int i)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null)
        {
            // Create a Connection to another node

            // ---- CONNECTION TO QUEST NODE ----
            if (inPoint.Parent is Quest)
            {
                Link link = Links[i];
                link.NextQuest = (Quest)inPoint.Parent;
                Links[i] = link;
            }

            if (inPoint.Parent is DialogueNode)
                PrecedingStartNode = (inPoint.Parent as DialogueNode);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
            ConnectionPoint.selectedOutPoint = OutPoint;
    }

    public override void Draw(Vector2 offset, int state)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        OutPoint.Draw();
        if (state == 0) InPoint.Draw();
        base.Draw(offset);
        GUI.Label(rect, Title, LabelStyle);
    }

    public override void ProcessEvent(Event e, int state, List<StoryNode> relatedNodes = null)
    {
        // ---- CONECTION POINT EVENTS ----
        InPoint.ProcessEvent(e);
        OutPoint.ProcessEvent(e);

        // ---- BASE EVENTS ----
        base.ProcessEvent(e, state, relatedNodes);

        // disable dragging in DialogueView
        if (state == 1) isDragging = false;
        
        // ---- ADDITIONAL CLICK EVENTS ----
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && rect.Contains(e.mousePosition))
                {
                    // Connect pending OutPoint
                    if (ConnectionPoint.selectedOutPoint != null)
                        OnInPointClick();
                    e.Use();
                }
                break;
        }
    }

    public override void FillContextMenu(GenericMenu contextMenu)
    {
        contextMenu.AddItem(new GUIContent("Open Dialogue"), false, () => ViewDialogue?.Invoke(this));
        base.FillContextMenu(contextMenu);
    }
    public override void Remove()
    {
        foreach(DialogueNode node in DialogueNodes.ToList())
            node.Remove();

        Customer.Quests.Remove(this);
        base.Remove();
        AssetDatabase.SaveAssets();
    }
    public override List<Connection> GetOutConnections(int state)
    {
        List<Connection> connections = new List<Connection>();
        switch (state)
        {
            // ---- CONNECTIONS IN QUESTVIEW ----
            case 0:
                foreach(Quest quest in Links.Where(e => e.NextQuest).Select(e => e.NextQuest))
                {
                    connections.Add(new Connection(quest.InPoint.Center, OutPoint.Center));
                }
                break;
            
            // ---- CONNECTION IN DIALOGUEVIEW ----
            case 1:
                if (PrecedingStartNode != null)
                    connections.Add(new Connection(PrecedingStartNode.InPoint.Center, OutPoint.Center, () => PrecedingStartNode = null));
                break;
        }
        return connections;
    }
}
