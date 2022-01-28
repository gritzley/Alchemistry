using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class PotionBranch : DialogueNode
{
    [System.Serializable]
    public struct Link
    {
        public PotionDefinition Potion;
        public DialogueNode NextNode;
    }
    public List<Link> Links;
    public List<Link> FilteredLinks;
    
    public override DialogueLine NextLine
    {
        get
        {
            if (GameManager.Instance.CurrentCustomer.LastGivenPotion != null)
            {
                foreach (Link link in Links)
                {
                    if (GameManager.Instance.CurrentCustomer.LastGivenPotion == link.Potion)
                    {
                        return link.NextNode.NextLine;
                    }
                }
                throw new NullReferenceException("You have given the character a potion that does not exist? How did you even do that?");
            }
            else {
                throw new NullReferenceException("There is no last given potion yet you are trying to access it.");
            }
        }
    }
    public PotionBranch()
    {
        Links = new List<Link>();
    }

#if UNITY_EDITOR
    [NonSerialized] public List<ConnectionPoint> OutPoints;
    public override void OnEnable()
    {
        Title = "Potion Branch";
        UpdateLinks();
        base.OnEnable();
    }

    public override List<Connection> GetOutConnections(int state = 0)
    {
        List<Connection> connections = new List<Connection>();
        foreach(Link link in FilteredLinks)
        {
            if (link.NextNode != null)
            {
                int i = FilteredLinks.IndexOf(link);
                connections.Add(new Connection(link.NextNode.InPoint.Center, OutPoints[i].Center, () => ClickConnection(i) ));
            }
        }
        return connections;
    }

    private void ClickConnection(int i)
    {
        Link link = FilteredLinks[i];
        link.NextNode = null;
        FilteredLinks[i] = link;
    }

    public override void OnOutPointClick(int i)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

       if (inPoint != null)
        {
            // Create a Connection to another node

            // ---- CONNECTION TO QUEST NODE ----
            if (inPoint.Parent is DialogueNode)
            {
                Link link = FilteredLinks[i];
                link.NextNode = (DialogueNode)inPoint.Parent;
                FilteredLinks[i] = link;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }

            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
        {
            ConnectionPoint.selectedOutPoint = OutPoints[i];
        }
    }

    public override void Draw(Vector2 offset, int state = 0)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;

        InPoint.Draw();
        // ---- NODE UNFOLDED ----
        OutPoints.ForEach( e => e.Draw() );
        FilteredLinks.ForEach( e => Size.x = Mathf.Max(Size.x, LabelStyle.CalcSize(new GUIContent(e.Potion.name)).x) );
        Size.y = 15 + OutPoints.Count * 25; 
        base.Draw(offset);
        for (int i = 0; i < FilteredLinks.Count; i++)
        {
            Rect labelRect = rect;
            labelRect.position += new Vector2(0, 25 * i);
            GUI.Label(labelRect, FilteredLinks[i].Potion.name, LabelStyle);
        }
    }

    public override void FillContextMenu(GenericMenu contextMenu)
    {
        base.FillContextMenu(contextMenu);
    }

    public override void ProcessEvent(Event e, int state = 0, List<StoryNode> relatedNodes = null)
    {
        // ---- CONECTION POINT EVENTS ----
        InPoint.ProcessEvent(e);
        OutPoints.ForEach( p => p.ProcessEvent(e));

        // ---- BASE EVENTS ----
        base.ProcessEvent(e, state, relatedNodes);
        
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
                    e.Use();
                }
                break;
        }
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        // Get all Potion assets and put them in a list
        List<PotionDefinition> potions = PotionDefinition.GetAllPotionAssets();
        
        Links.RemoveAll( e => !potions.Contains(e.Potion));

        potions
        .Except( Links.Where(e => potions.Contains(e.Potion)).Select(e => e.Potion) )
        .ToList()
        .ForEach( e => Links.Add(new Link() {Potion = e}));     

        List<PotionDefinition> PossiblePotions = new List<PotionDefinition>();
        List<Quest> OtherCustomerQuests = ParentQuest.Customer.Quests.Where(e => e != this).ToList();
        foreach (Quest quest in OtherCustomerQuests) 
        {
            foreach (Quest.Link link in quest.Links)
            {
                if (link.NextQuest == ParentQuest && !PossiblePotions.Contains(link.Potion))
                {
                    PossiblePotions.Add(link.Potion);
                }
            }
        }
        FilteredLinks = Links.Where(e => PossiblePotions.Contains(e.Potion)).ToList();

        OutPoints = new List<ConnectionPoint>();
        for (int i = 0; i < FilteredLinks.Count; i++)
        {
            OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick, i));
        }
    }

#endif

}