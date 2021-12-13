using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class StoryEditor : EditorWindow
{
    public enum ViewState
    {
        QuestView,
        DialogueView
    }
    Vector2 offset;
    ViewState viewState;
    bool isDragging;
    List<StoryNode> nodes;
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

    /// <summary>
    /// Initialize Window
    /// </summary>
    private void OnEnable()
    {
        offset = new Vector2(PlayerPrefs.GetFloat("StoryEditorOffsetX", 0), PlayerPrefs.GetFloat("StoryEditorOffsetY", 0));
        ViewQuests();
    }

    /// <summary>
    /// View all the Quest Assets in the Assets/Dialogue Path as Nodes
    /// </summary>
    private void ViewQuests()
    {
        viewState = ViewState.QuestView;
        // There is no simple way to get all assets of a certain type.
        // Instead we are getting a list of all internal references to quest assets in a list of directories
        // which we convert to actual paths, which we then use to load the actual assets.
        nodes = AssetDatabase.FindAssets("t:Quest", new string[] { "Assets/Dialogue" })
        .Select( e => {
            string path = AssetDatabase.GUIDToAssetPath(e);
            Quest quest = (Quest)AssetDatabase.LoadAssetAtPath(path, typeof(Quest));
            quest.OnRemove = RemoveNodeFromView;
            quest.ViewDialogue = ViewQuestDialogue;

            return (StoryNode)quest;
        })
        .ToList();
    }

    private void ViewQuestDialogue(Quest quest)
    {
        viewState = ViewState.DialogueView;
        nodes = new List<StoryNode>();
        nodes.Add(quest);
        nodes.AddRange(quest.Lines);
    }

    /// <summary>
    /// Removes a quest from the current node view
    /// </summary>
    /// <param name="quest"> The quest to be removed</param>
    private void RemoveNodeFromView(StoryNode node)
    {
        nodes.Remove(node);
    }

    /// <summary>
    /// Add a new Quest to the Assets/Dialogue/ directory
    /// </summary>
    /// <param name="position"></param>
    private void AddQuest(Vector2 position)
    {
        // GenerateUniqueAssetPath increments the name until a unused name is found
        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Dialogue/Quest.asset");
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        AssetDatabase.CreateAsset(quest, path);

        quest.Title = quest.name;
        quest.Position = position;
        quest.OnRemove = RemoveNodeFromView;
        quest.ViewDialogue = ViewQuestDialogue;

        EditorUtility.SetDirty(quest);
        AssetDatabase.SaveAssets();
        nodes.Add(quest);
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
        nodes.ForEach( e => e.Connections.ForEach( e => e.ProcessEvent(Event.current)));
        nodes.ForEach( e => e.ProcessEvent(Event.current));
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                // ---- WINDOW CLICK ----
                if (Event.current.button == 0)
                {
                    ConnectionPoint.selectedInPoint = null;
                    ConnectionPoint.selectedOutPoint = null;
                    questNodes.ForEach(e => e.isUnfolded = false );
                        
                    // Tell Unity to redraw the window 
                    GUI.changed = true;
                }

                // ---- WINDOW CONTEXT MENU ----
                if (Event.current.button == 1)
                {
                    
                    GenericMenu contextMenu = new GenericMenu();
                    // When clicking the menu item, the mosueposition does not exist anymore, becuase we are not technicaly in the window anymore.
                    // Therefore we must save the mouse position when creatigng the context menu.
                    Vector2 pos = Event.current.mousePosition - offset;
                    contextMenu.AddItem(new GUIContent("Add Quest"), false, () => AddQuest(pos));
                    contextMenu.ShowAsContext();
                }
                break;

            case EventType.MouseDrag:

                // ---- DRAG ----
                if (Event.current.button == 0)
                {
                    isDragging = true;
                    Vector2 drag = Event.current.delta;
                    offset += drag;

                    // Tell Unity to redraw the window
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
                break;
        }

        // ---- DRAW NODES & CONNECTIONS ----
        nodes.ForEach( e => e.Connections.ForEach( e => e.Draw()));
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

        if (GUI.changed) Repaint();
    }

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