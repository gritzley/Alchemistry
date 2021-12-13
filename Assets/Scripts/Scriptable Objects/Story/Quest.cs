using System;
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
    ConnectionPoint InPoint;
    List<ConnectionPoint> OutPoints;
    public List<DialogueLine> Lines;
    public bool isUnfolded;
    public Action<Quest> ViewDialogue;

    public override List<Connection> Connections
    {
        get 
        {
            List<Connection> connections = new List<Connection>();

            foreach(Link link in Links)
            {
                if (link.NextQuest != null)
                {
                    int i = Links.IndexOf(link);
                    connections.Add(new Connection(link.NextQuest.InPoint.Center, OutPoints[isUnfolded ? i : 0].Center, () => ClickConnection(i) ));
                }
            }

            return connections;
        }
    }

    public Quest() : base()
    {
        Links = new List<Link>();
        Lines = new List<DialogueLine>();
    }
    public override void OnEnable()
    {
        isUnfolded = false;
        base.OnEnable();
        UpdateLinks();
        InPoint = new ConnectionPoint(this, ConnectionPointType.In, SelectInPoint);
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        List<Potion> potions = AssetDatabase.FindAssets("t:Potion")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Potion)AssetDatabase.LoadAssetAtPath(e, typeof(Potion)))
        .ToList();

        Links.RemoveAll( e => !potions.Contains(e.Potion));

        potions
        .Except( Links.Where(e => potions.Contains(e.Potion)).Select(e => e.Potion) )
        .ToList()
        .ForEach( e => Links.Add(new Link() {Potion = e}));

        OutPoints = new List<ConnectionPoint>();
        for (int i = 0; i < Links.Count; i++)
        {
            OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, SelectLinkOutPoint, i));
        }
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
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            ConnectionPoint.selectedOutPoint = OutPoints[i];
        }
    }

    private void SelectInPoint(int i = 0)
    {
        ConnectionPoint outPoint = ConnectionPoint.selectedOutPoint;
        // 
        if (outPoint != null)
        {
            if (outPoint.Parent is Quest)
            {
                Quest.Link link = ((Quest)outPoint.Parent).Links[outPoint.Index];
                link.NextQuest = this;
                ((Quest)outPoint.Parent).Links[outPoint.Index] = link;
                ConnectionPoint.selectedOutPoint = null;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            ConnectionPoint.selectedInPoint = InPoint;
        }
    }

    public void ClickConnection(int i)
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

            case 1:
                OutPoints[0].Draw();
                OutPoints[1].Draw();
                Size.y = 65;
                base.Draw(offset);
                break;
        }
    }

    public override void ProcessEvent(Event e)
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
        switch(e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && rect.Contains(e.mousePosition))
                {
                    // Connect pending OutPoint
                    if (ConnectionPoint.selectedOutPoint != null)
                    {
                        SelectInPoint();
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
}
