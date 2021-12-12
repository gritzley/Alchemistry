using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quest : StoryNode
{

    // ---- STORY TREE ----
    // The First Line of the dialogue before the choice of potion
    public DialogueLine PrecedingStartLine;
    // The first line of the dialogue after the choice of potion
    public DialogueLine SucceedingStartLine;
    // The current Line
    public DialogueLine CurrentLine;

    // ---- LINKS ----
    // Struct for Links, that link a Potion to a new quest
    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    // A List of Links for this quest
    public List<Link> Links;

    // ---- EDITOR ----
    // A reference to all lines that are associatec with this quest, even if not connected to the dialogue tree
    List<DialogueLine> Lines;
    ConnectionPoint InPoint;
    List<ConnectionPoint> OutPoints;

    public bool isUnfolded;

    public override List<Connection> Connections
    {
        get
        {
            return Links
            .Where( e => e.NextQuest != null )
            .Select( (e, i) => new Connection(e.NextQuest.InPoint.Center, OutPoints[isUnfolded ? i : 0].Center, () => {
                if (isUnfolded) {
                    Link link = Links[i];
                    link.NextQuest = null;
                    Links[i] = link;
                }
                else
                {
                    isUnfolded = true;
                }
            }))
            .ToList();
        }
    }

    // Constructor
    public Quest() : base()
    {
        // Init Lists
        Links = new List<Link>();
        Lines = new List<DialogueLine>();
    }
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateLinks();
        InPoint = new ConnectionPoint(this, ConnectionPointType.In, () => { SelectInPoint(); });
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        // Get all Potion assets and put them in a list
        List<Potion> potions = AssetDatabase.FindAssets("t:Potion")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Potion)AssetDatabase.LoadAssetAtPath(e, typeof(Potion)))
        .ToList();

        // Craete a new Link list 
        List<Link> links = new List<Link>();

        // Add old Links if the potion still exists
        foreach (Link link in Links)
        {
            if (potions.Contains(link.Potion))
            {
                links.Add(link);
                // Remove the potions from the potion list, we made earlier
                potions.Remove(link.Potion);
            }
        }
        
        // Each Potion that is still in the list, does not have a link in the potion
        foreach (Potion potion in potions)
        {
            // Add links to these potions that point to nothing 
            Link link = new Link();
            link.Potion = potion;
            links.Add(link);
        }

        // Set this Quests Links to the new links
        Links = links;

        // Create enough Out points for all potions
        OutPoints = Links
        .Select( (e, i) => new ConnectionPoint(this, ConnectionPointType.Out, () => { SelectLinkOutPoint(i); }, i))
        .ToList();
    }

    private void SelectLinkOutPoint(int i)
    {
        if (!isUnfolded) {
            isUnfolded = true;
        }
        else if (ConnectionPoint.selectedInPoint != null)
        {
            if (ConnectionPoint.selectedInPoint.Parent is Quest)
            {
                Link link = Links[i];
                link.NextQuest = (Quest)ConnectionPoint.selectedInPoint.Parent;
                Links[i] = link;
                ConnectionPoint.selectedInPoint = null;
            }
        }
        else
        {
            ConnectionPoint.selectedOutPoint = OutPoints[i];
        }
    }

    private void SelectInPoint()
    {
        ConnectionPoint outPoint = ConnectionPoint.selectedOutPoint;
        if (outPoint != null)
        {
            if (outPoint.Parent is Quest)
            {
                Quest.Link link = ((Quest)outPoint.Parent).Links[outPoint.Index];
                link.NextQuest = this;
                ((Quest)outPoint.Parent).Links[outPoint.Index] = link;
                ConnectionPoint.selectedOutPoint = null;
            }
        }
        else
        {
            ConnectionPoint.selectedInPoint = InPoint;
        }
    }

    public override void Draw(Vector2 offset)
    {
        InPoint.Draw();
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;

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
    }

    public override void ProcessEvent(Event e)
    {
        InPoint.ProcessEvent(e);
        if (isUnfolded)
        {
            OutPoints.ForEach( p => p.ProcessEvent(e));
        }
        else
        {
            OutPoints[0].ProcessEvent(e);
        }
        base.ProcessEvent(e);
        switch(e.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(e.mousePosition))
                {
                    if (ConnectionPoint.selectedOutPoint != null)
                    {
                        SelectInPoint();
                        e.Use();
                    }
                    else
                    {
                        isUnfolded = true;
                        GUI.changed = true;
                        e.Use();
                    }
                }
                break;
        }
    }
}
