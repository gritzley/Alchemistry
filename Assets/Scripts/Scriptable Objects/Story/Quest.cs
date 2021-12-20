using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quest : StoryNode
{

    // ---- STORY TREE ----
    public DialogueNode PrecedingStartNode;
    public DialogueNode SucceedingStartNode;
    public DialogueLine CurrentLine;
    public CharacterController ParentCharacter;

    // ---- LINKS ----
    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    // ---- EDITOR ----
    List<ConnectionPoint> OutPoints;
    public List<DialogueNode> DialogueNodes;
    public bool isUnfolded;
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
        isUnfolded = false;
        base.OnEnable();
        UpdateLinks();
        CurrentLine = PrecedingStartNode?.NextLine;
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        List<Potion> potions = Potion.GetAllPotionAssets();

        Links.RemoveAll( e => !potions.Contains(e.Potion));

        potions
        .Except( Links.Where(e => potions.Contains(e.Potion)).Select(e => e.Potion) )
        .ToList()
        .ForEach( e => Links.Add(new Link() {Potion = e}));

        OutPoints = new List<ConnectionPoint>();
        for (int i = 0; i < Links.Count; i++)
        {
            OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick, i));
        }
    }

    /// <summary>
    /// Action to be passed to a Connection between two quests as an OnClick Handle
    /// </summary>
    /// <param name="i">The index of the Link corresponding to the node</param>
    public void ClickQuestConnection(int i)
    {
        if (isUnfolded) {
            Link link = Links[i];
            link.NextQuest = null;
            Links[i] = link;
        }
        else
        {
            isUnfolded = true;
        }
    }

    public override void OnOutPointClick(int i)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (!isUnfolded) {
            isUnfolded = true;
        }
        else if (inPoint != null)
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
            {
                switch (i)
                    {
                        // Connect to Right
                        case 0:
                            PrecedingStartNode = (inPoint.Parent as DialogueNode);
                            break;
                        // Connect to Left
                        case 1:
                            SucceedingStartNode = (inPoint.Parent as DialogueNode);
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
            ConnectionPoint.selectedOutPoint = OutPoints[i];
        }
    }

    public override void Draw(Vector2 offset, int state)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        switch (state)
        {
            // ---- QUEST VIEW ----
            case 0:
                InPoint.Draw();

                // ---- NODE UNFOLDED ----
                if (isUnfolded)
                {
                    OutPoints.ForEach( e => e.Draw() );
                    Links.ForEach( e => Size.x = Mathf.Max(Size.x, LabelStyle.CalcSize(new GUIContent(e.Potion.name)).x) );
                    Size.y = 15 + OutPoints.Count * 25; 
                    base.Draw(offset);
                    for (int i = 0; i < Links.Count; i++)
                    {
                        Rect labelRect = rect;
                        labelRect.position += new Vector2(0, 25 * i);
                        GUI.Label(labelRect, Links[i].Potion.name, LabelStyle);
                    }
                }
                /// ---- NODE FOLDED ----
                else
                {
                    OutPoints[0].Draw();
                    Size.y = 40;
                    base.Draw(offset);
                    GUI.Label(rect, Title, LabelStyle);
                }
                break;

            // ---- DIALOGUE VIEW ----
            case 1:
                OutPoints[0].Draw();
                OutPoints[1].Draw();
                Size.y = 65;
                base.Draw(offset);
                GUI.Label(rect, Title, LabelStyle);
                break;
        }
    }

    public override void ProcessEvent(Event e, int state)
    {
        // ---- CONECTION POINT EVENTS ----
        InPoint.ProcessEvent(e);
        if (isUnfolded)
        {
            OutPoints.ForEach( p => p.ProcessEvent(e));
        }
        else
        {
            OutPoints[0].ProcessEvent(e);
        }

        // ---- BASE EVENTS ----
        base.ProcessEvent(e);

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
                    {
                        OnInPointClick();
                    }
                    // Unfold Node
                    else
                    {
                        isUnfolded = true;
                        GUI.changed = true;
                    }
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
        foreach(DialogueNode node in DialogueNodes)
        {
            node.Remove();
        }
        base.Remove();
    }
    public override List<Connection> GetOutConnections(int state)
    {
        List<Connection> connections = new List<Connection>();
        switch (state)
        {
            // ---- CONNECTIONS IN QUESTVIEW ----
            case 0: 
                foreach(Link link in Links)
                {
                    if (link.NextQuest != null)
                    {
                        int i = Links.IndexOf(link);
                        connections.Add(new Connection(link.NextQuest.InPoint.Center, OutPoints[isUnfolded ? i : 0].Center, () => ClickQuestConnection(i) ));
                    }
                }
                break;
            
            // ---- CONNECTION IN DIALOGUEVIEW ----
            case 1:
                if (PrecedingStartNode != null)
                {
                    connections.Add(new Connection(PrecedingStartNode.InPoint.Center, OutPoints[0].Center, () => PrecedingStartNode = null));
                }
                if (SucceedingStartNode != null)
                {
                    connections.Add(new Connection(SucceedingStartNode.InPoint.Center, OutPoints[1].Center, () => SucceedingStartNode = null));
                }
                break;
        }
        return connections;
    }
}
