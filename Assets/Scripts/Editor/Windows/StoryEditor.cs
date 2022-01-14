using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class StoryEditor : EditorWindow
{
    public static StoryEditor Instance;
    public static bool IsOpen;
    public enum ViewState
    {
        QuestView,
        DialogueView
    }
    Vector2 selectCornerStart, selectCornerEnd;
    Vector2 selectBoxPos => new Vector2(Mathf.Min(selectCornerEnd.x, selectCornerStart.x), Mathf.Min(selectCornerEnd.y, selectCornerStart.y));
    Vector2 selectBoxSize => new Vector2(Mathf.Abs(selectCornerEnd.x - selectCornerStart.x), Mathf.Abs(selectCornerEnd.y - selectCornerStart.y));
    Rect selectionRect => new Rect(selectBoxPos, selectBoxSize);
    bool drawingSelectionBox;
    Vector2 offset;
    ViewState viewState;
    bool isDragging;
    List<StoryNode> nodes, selectedNodes;
    List<Quest> questNodes { get { return nodes.Where( e => e is Quest).Select( e => e as Quest).ToList(); } }

    /// <summary>
    /// Add a Menu Item to open the window
    /// </summary>
    [MenuItem("Window/Story Editor")]
    private static void OpenWindow()
    {
        // Create an instance of the window and set the title
        StoryEditor window = GetWindow<StoryEditor>();
        window.titleContent = new GUIContent("Story Editor");
    }

    private void OnEnable()
    {
        Assert.IsNull(Instance, "There can only be one open Story Editor at any time");
        Instance = this;
        if (GameManager.Instance == null)
            Debug.LogWarning("There is no GameManager Instance!");
        else if (GameManager.Instance.CurrentCustomer == null)
            Debug.LogWarning("There is no test_character assigned in the GameManager");
        else
        {
            ViewQuests();
            offset.x = PlayerPrefs.GetFloat("StoryEditorOffsetX");
            offset.y = PlayerPrefs.GetFloat("StoryEditorOffsetY");
        }
        selectedNodes = new List<StoryNode>();
    }

    private void OnDisable()
    {
        Instance = null;
    }

    /// <summary>
    /// View all the Quest Assets in the Assets/Dialogue Path as Nodes
    /// </summary>
    private void ViewQuests()
    {
        viewState = ViewState.QuestView;
        nodes = GameManager.Instance.CurrentCustomer.CustomerDefinition.Quests.Select( quest => {
            quest.OnRemove = RemoveNodeFromView;
            quest.ViewDialogue = ViewQuestDialogue;
            return (StoryNode)quest;
        }).ToList();
        offset = Vector2.zero;

    }

    private void ViewQuestDialogue(Quest quest)
    {
        viewState = ViewState.DialogueView;
        nodes = new List<StoryNode>();
        nodes.Add(quest);
        nodes.AddRange(quest.DialogueNodes);
        quest.DialogueNodes.ForEach(e => e.OnRemove = RemoveNodeFromView);
    }

    /// <summary>
    /// Removes a quest from the current node view
    /// </summary>
    /// <param name="quest"> The quest to be removed</param>
    private void RemoveNodeFromView(StoryNode node)
    {
        DeselectAllNodes();
        nodes.Remove(node);
    }

    /// <summary>
    /// Add a new Quest to the Assets/Dialogue/ directory
    /// This Method assumes that you are in QuestView
    /// </summary>
    /// <param name="position"></param>
    private void AddQuest(Vector2 position)
    {
        if (viewState != ViewState.QuestView) throw new Exception("You tried to add a new Quest even though you are not in QuestView");
        // GenerateUniqueAssetPath increments the name until a unused name is found
        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Dialogue/Quest.asset");
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        AssetDatabase.CreateAsset(quest, path);

        // We need to add some parameters of the quest because scriptableObjets don't pass stuff to constructors
        quest.Title = quest.name;
        quest.Position = position;
        quest.OnRemove = RemoveNodeFromView;
        quest.ViewDialogue = ViewQuestDialogue;
        quest.Customer = GameManager.Instance.CurrentCustomer.CustomerDefinition;
        GameManager.Instance.CurrentCustomer.CustomerDefinition.Quests.Add(quest);

        // Changes made to the line after creating it must be saved
        EditorUtility.SetDirty(quest);
        EditorUtility.SetDirty(GameManager.Instance.CurrentCustomer.CustomerDefinition);
        AssetDatabase.SaveAssets();
        nodes.Add(quest);
    }

    private void CreateDialogueNode<T>(Vector2 position) where T : DialogueNode
    {
        if (viewState != ViewState.DialogueView) throw new Exception("You tried to add a new DialogueNode even though you are not in DialogueViews");
        Quest currentQuest = (Quest)nodes[0];

        string assetName;
        switch (typeof(T).Name)
        {
            case "DialogueLine": assetName = "Line"; break;
            case "PotionBranch": assetName = "Branch"; break;
            default: throw new NotImplementedException("You are trying to create a type of node that the Story Editor does not know about");
        }

        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Dialogue/{assetName}.asset");
        T node = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(node, path);

        node.Title = node.name;
        node.Position = position;
        node.OnRemove = RemoveNodeFromView;
        node.ParentQuest = currentQuest;
        currentQuest.DialogueNodes.Add(node);

        // Changes made to the line after creating it must be saved
        EditorUtility.SetDirty(node);
        EditorUtility.SetDirty(currentQuest);
        AssetDatabase.SaveAssets();
        nodes.Add(node);
    }

    private void GoToMiddleOfNodes()
    {
        offset = Vector2.zero;
        foreach(StoryNode node in nodes)
        {
            offset += node.Position;
        }
        offset /= nodes.Count;
        offset -= position.size / 2;
        offset *= -1;
    }

    private void GoToFirstNode()
    {
        offset = nodes.First().Position;
        offset -= position.size / 2;
        offset *= -1;
    }

    private void GoToLastNode()
    {
        offset = nodes.Last().Position;
        offset -= position.size / 2;
        offset *= -1;
    }

    /// <summary>
    /// OnGUI is called whenever the window is being drawn. This is essentially the main function of the Window
    /// </summary>
    private void OnGUI()
    {
        // ---- DRAW GRID ----
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        // ---- PROCESS EVENTS ----

        nodes.ForEach( e => e.GetOutConnections((int)viewState).ForEach( e => e.ProcessEvent(Event.current)));
        nodes.Except(selectedNodes).ToList().ForEach( e => e.ProcessEvent(Event.current, (int)viewState));
        selectedNodes.ForEach(node => node.ProcessEvent(Event.current, (int)viewState, selectedNodes.Where(e => e != node).ToList()));
        if (!selectedNodes.Exists(e => e.rect.Contains(Event.current.mousePosition)))
        {
            switch (Event.current.type)
            {

                case EventType.MouseDown:
                    // ---- WINDOW CLICK ----
                    if (Event.current.button == 0)
                    {
                        ConnectionPoint.selectedInPoint = null;
                        ConnectionPoint.selectedOutPoint = null;
                            
                        // Tell Unity to redraw the window 
                        GUI.changed = true;

                        selectCornerStart = Event.current.mousePosition;
                        selectCornerEnd = Event.current.mousePosition;
                        drawingSelectionBox = true;
                    }

                    // ---- WINDOW CONTEXT MENU ----
                    if (Event.current.button == 1)
                    {
                        
                        GenericMenu contextMenu = new GenericMenu();
                        // When clicking the menu item, the mosueposition does not exist anymore, becuase we are not technicaly in the window anymore.
                        // Therefore we must save the mouse position when creatigng the context menu.
                        Vector2 pos = Event.current.mousePosition - offset;
                        switch(viewState)
                        {
                            case ViewState.DialogueView:
                                contextMenu.AddItem(new GUIContent("Add Line"), false, () => CreateDialogueNode<DialogueLine>(pos));
                                contextMenu.AddItem(new GUIContent("Add Potion Branch"), false, () => CreateDialogueNode<PotionBranch>(pos));
                        
                                contextMenu.AddItem(new GUIContent("Return to Quest View"), false, ViewQuests);
                                contextMenu.AddItem(new GUIContent("I am lost, go back to the start"), false, GoToFirstNode);
                                contextMenu.AddItem(new GUIContent("I am lost, go to the last node I added"), false, GoToLastNode);
                                break;

                            case ViewState.QuestView:
                                contextMenu.AddItem(new GUIContent("Add Quest"), false, () => AddQuest(pos));
                                contextMenu.AddItem(new GUIContent("I am lost, take me back to the nodes"), false, GoToMiddleOfNodes);
                                break;
                        }
                        contextMenu.ShowAsContext();
                    }
                    break;

                case EventType.MouseDrag:

                    // ---- DRAG ----
                    if (Event.current.button == 2)
                    {
                        isDragging = true;
                        Vector2 drag = Event.current.delta;
                        offset += drag;

                        // Tell Unity to redraw the window
                        GUI.changed = true;
                    }
                    if (Event.current.button == 0)
                    {
                        selectCornerEnd = Event.current.mousePosition;
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseUp:

                    // ---- DRAG END ----
                    if (isDragging)
                    {
                        isDragging = false;
                        PlayerPrefs.SetFloat("StoryEditorOffsetX", offset.x);
                        PlayerPrefs.SetFloat("StoryEditorOffsetY", offset.y);
                    }
                    DeselectAllNodes();
                    HandleSelection();
                    break;
            }
        }

        // ---- DRAW NODES & GetConnections((int)viewState) ----
        nodes.ForEach( e => e.GetOutConnections((int)viewState).ForEach( e => e.Draw()));
        nodes.ForEach( e => e.Draw(offset, (int)viewState) );

        if (ConnectionPoint.selectedInPoint != null && ConnectionPoint.selectedOutPoint == null)
        {
            new Connection(ConnectionPoint.selectedInPoint.Center, Event.current.mousePosition).Draw();
            
            // Tell Unity to redraw the window
            GUI.changed = true;
        }
        if (ConnectionPoint.selectedOutPoint != null && ConnectionPoint.selectedInPoint == null)
        {
            new Connection(Event.current.mousePosition, ConnectionPoint.selectedOutPoint.Center).Draw();
            
            // Tell Unity to redraw the window
            GUI.changed = true;
        }

        // ---- DRAW SELECTION BOX ----
        if (drawingSelectionBox)
        {
            GUI.Box(new Rect(selectCornerStart, selectCornerEnd - selectCornerStart), "");
        }

        if (GUI.changed) Repaint();
    }

    private void HandleSelection()
    {
        if (drawingSelectionBox)
        {
            selectedNodes = nodes.Where(e => selectionRect.Overlaps(e.rect)).ToList();
            selectedNodes.ForEach(e => e.isSelected = true);
            if (selectedNodes.Count == 1)
            {
                Selection.activeObject = selectedNodes[0];
            }
            if (selectedNodes.Count > 1)
            {
                Selection.activeObject = null;
            }
        }

        drawingSelectionBox = false;
        GUI.changed = true;

    }

    private void DeselectAllNodes() =>
        nodes.Where(e => Selection.activeObject != e)
        .ToList().ForEach(e => e.isSelected = false);

    /// <summary>
    /// Draw a grid in the window
    /// </summary>
    /// <param name="gridSpacing">The spacing between the grids line</param>
    /// <param name="gridOpacity">The opacity of the grids lines</param>
    /// <param name="gridColor">The color of the girds lines</param>
    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector3 gridPosition = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + gridPosition, new Vector3(gridSpacing * i, position.height, 0f) + gridPosition);
        }
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + gridPosition, new Vector3(position.width, gridSpacing * j, 0f) + gridPosition);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}