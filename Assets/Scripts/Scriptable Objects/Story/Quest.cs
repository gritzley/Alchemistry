using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quest : SceneNode
{

    // ---- STORY TREE ----
    public DialogueNode PrecedingStartNode;
    private DialogueLine _currentLine;
    public DialogueLine CurrentLine
    {
        get
        {
            if (_currentLine == null) _currentLine = PrecedingStartNode?.NextLine;
            return _currentLine;
        }
        set => _currentLine = value;
    }

    public bool HasReceivingState;

    // ---- LINKS ----
    [System.Serializable]
    public struct Link
    {
        public PotionDefinition Potion;
        public SceneNode NextQuest;
    }

    public SceneNode NextScene;

    public List<Link> Links;

#if UNITY_EDITOR
    // ---- EDITOR ----
    [NonSerialized] private ConnectionPoint OutPoint;
    public List<DialogueNode> DialogueNodes;
    [NonSerialized] public Action<Quest> ViewDialogue;
    [SerializeField] private bool isProofread;

#endif
    
    // CONSTRUCTOR
    public Quest() : base()
    {
        Links = new List<Link>();

#if UNITY_EDITOR
        DialogueNodes = new List<DialogueNode>();
#endif

    }

#if !UNITY_EDITOR

    public void OnEnable()
    {
        CurrentLine = PrecedingStartNode.NextLine;
    }
#endif

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

    public SceneNode GetNextScene(PotionDefinition potion)
    {
        if (HasReceivingState)
            return Links.Find(e => e.Potion == potion).NextQuest;
        else
            return NextScene;
    }


#if UNITY_EDITOR

    /// <summary>
    /// Set initial Values when enabling this quest
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateLinks();
        Size.y = 40;
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick);
        DialogueNodes = DialogueNodes.Where(e => e != null).ToList(); // thus the grand culling began
        HasReceivingState = DialogueNodes.Exists(e => e != null && e is DialogueLine && (e as DialogueLine).IsReceivingState);
        CurrentLine = PrecedingStartNode.NextLine;
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

    public override void OnOutPointClick(int i)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null && inPoint.Parent is DialogueNode)
        {
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

        if (!isProofread) DrawNotification(1);
        bool hasEmptyLinks = Links.Exists(e => e.NextQuest == null || !(Customer.Quests.Contains(e.NextQuest) || Customer.Articles.Contains(e.NextQuest)));
        if (hasEmptyLinks && HasReceivingState) DrawNotification(2);
        bool hasUnconnectedPotionBranch = DialogueNodes.Exists(e => e is PotionBranch && (e as PotionBranch).FilteredLinks.Exists(e => e.NextNode == null));
        if (hasUnconnectedPotionBranch) DrawNotification(2);
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
            node?.Remove();

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
                if (HasReceivingState)
                    foreach(SceneNode quest in Links.Where(e => e.NextQuest).Select(e => e.NextQuest))
                        connections.Add(new Connection(quest.InPoint.Center, OutPoint.Center));
                else
                    if (NextScene != null)
                        connections.Add(new Connection(NextScene.InPoint.Center, OutPoint.Center));
                break;
            
            // ---- CONNECTION IN DIALOGUEVIEW ----
            case 1:
                if (PrecedingStartNode != null)
                    connections.Add(new Connection(PrecedingStartNode.InPoint.Center, OutPoint.Center, () => PrecedingStartNode = null));
                break;
        }
        return connections;
    }
#endif

}
