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

    public override List<Connection> Connections
    {
        get
        {
            return Links
            .Where( e => e.NextQuest != null )
            .Select( (e, i) => new Connection(e.NextQuest.InPoint.Center, OutPoints[isSelected ? i : 0].Center))
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
        InPoint = new ConnectionPoint(this, ConnectionPointType.In);
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

        OutPoints = Links
        .Select( (e, i) => new ConnectionPoint(this, ConnectionPointType.Out, i))
        .ToList();
    }

    public override void Draw(Vector2 offset)
    {
        InPoint.Draw();
        if (isSelected)
        {
            OutPoints.ForEach( e => e.Draw() );
            Size.x = 0;
            Links.ForEach( e => Size.x = Mathf.Max(Size.x, LabelStyle.CalcSize(new GUIContent(Title)).x) );
            Size.y = 40 + Links.Count * 25; 
            base.Draw(offset);
            LabelStyle.alignment = TextAnchor.UpperRight;
            for (int i = 0; i < Links.Count; i++)
            {
                Rect labelRect = rect;
                labelRect.position += new Vector2(0, 25 * i);
                GUI.Label(labelRect, Links[i].Potion.name, LabelStyle);
            }
        }
        else
        {
            OutPoints[0].Draw();
            Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
            Size.y = 40; 
            base.Draw(offset);
            LabelStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(rect, Title, LabelStyle);
        }
    }
}
