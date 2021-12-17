using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PotionBranch : DialogueNode
{
    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public DialogueLine NextLine;
    }
    public List<Link> Links;

    public List<ConnectionPoint> OutPoints;
    
    public bool isUnfolded;

    public PotionBranch()
    {
        Links = new List<Link>();
    }
    public override void OnEnable()
    {
        Title = "Potion Branch";
        UpdateLinks();
        base.OnEnable();
    }

    public override List<Connection> GetOutConnections(int state = 0)
    {
        List<Connection> connections = new List<Connection>();
        foreach(Link link in Links)
        {
            if (link.NextLine != null)
            {
                int i = Links.IndexOf(link);
                connections.Add(new Connection(link.NextLine.InPoint.Center, OutPoints[isUnfolded ? i : 0].Center, () => ClickConnection(i) ));
            }
        }
        return connections;
    }

    private void ClickConnection(int i)
    {
        if (isUnfolded)
        {
            Link link = Links[i];
            link.NextLine = null;
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
            if (inPoint.Parent is DialogueLine)
            {
                Link link = Links[i];
                link.NextLine = (DialogueLine)inPoint.Parent;
                Links[i] = link;
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

    public override void FillContextMenu(GenericMenu contextMenu)
    {
        base.FillContextMenu(contextMenu);
    }

    public override void ProcessEvent(Event e, int state = 0)
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

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        // Get all Potion assets and put them in a list
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
}