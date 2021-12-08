using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class DialogueEditor : EditorWindow
{
    // An enum to symbolize the different states the window can be in
    public enum WindowState {
        QuestView,
        DialogueView
    }
    // The current state of the window
    public WindowState State;

    // A static instance of the DialogueEditor to address it from the outside
    public static DialogueEditor Instance { get; private set; }
    // Shows whether there is an open instance of this window
    public static bool IsOpen {
        get { return Instance != null; }
    }

    // The currently displayed nodes and connections
    private List<Node> nodes;
    private List<Connection> connections;

    // References to dialogueNodes and questNodes. These may overlap with the "main" nodes list.
    // If a deeper level of editor is displayed, higher level nodes are preserved in these lists for quick loading.
    private List<DialogueLineNode> dialogueLineNodes;
    private List<QuestNode> questNodes;

    // If the window is in dialogueView, this holds a reference to the questNode the current dialogue derives from
    private QuestNode currentQuestNode;

    // Style References
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    // Node data objects
    private NodeData defaultNodeData;
    private NodeData dialogueLineNodeData;
    private NodeData questNodeData;

    // References to the currently selected points
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    // Out current position in the grid
    private Vector2 offset;
    // The amount the window has been dragged since the last repaint
    private Vector2 drag;

    // Quick reference to the Dialogue Path
    private string dialoguePath = "Assets/Dialogue";

    // A simple flag to see if you are dragging the editor
    private bool isDragging;

    /// <summary>
    /// OnepWindow is called when the user is selecting the window in the Editors Open Window dialogue
    /// </summary>
    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow()
    {
        // Create an instance of the window and set the title
        DialogueEditor window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    /// <summary>
    /// OnEnable is called when the window is opened or editor is reloaded
    /// </summary>
    private void OnEnable()
    {
        // Instantiate the Editor
        Instance = this;

        // Initialize default node data
        defaultNodeData = new NodeData();
        defaultNodeData.OnClickInPoint = OnClickInPoint;
        defaultNodeData.OnClickOutPoint = OnClickOutPoint;
        defaultNodeData.OnClickRemoveNode = RemoveNode;

        // Collect Quest Assets
        Quest[] quests = AssetDatabase.FindAssets("t:Quest", new string[] { dialoguePath })
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Quest)AssetDatabase.LoadAssetAtPath(e, typeof(Quest)))
        .ToArray();

        // Show the quests
        ShowQuests(quests);
    }

    /// <summary>
    /// Called when the Editor Window is disabled
    /// </summary>
    private void OnDisable()
    {
        AssetDatabase.SaveAssets();
        // Deinstantiate
        if (IsOpen) Instance = null;
    }

    /// <summary>
    /// Show all quests from an array
    /// </summary>
    /// <param name="quests">An arra of quests</param>
    private void ShowQuests(Quest[] quests)
    {
        // Set Window State
        State = WindowState.QuestView;

        // Reset Nodes and Connections
        nodes = new List<Node>();
        connections = new List<Connection>();
        questNodes = new List<QuestNode>();
        dialogueLineNodes = new List<DialogueLineNode>();

        // Create QuestNodes from Quest Assets
        foreach (Quest quest in quests)
        {
            QuestNode node = new QuestNode(quest, defaultNodeData, ShowQuestDialogue, OnQuestNodeDragEnd);
            questNodes.Add(node);
            nodes.Add((Node)node);
        }

        // Draw the connections
        CreateQuestNodeConnections();
    }

    /// <summary>
    /// Draw the connections between all displayed questNodes
    /// </summary>
    private void CreateQuestNodeConnections()
    {
        // Loop through all links in all questNodes
        foreach (QuestNode node in questNodes)
        {
            for (int i = 0; i < node.Quest.Links.Count; i++)
            {
                // Get a reference to the quest the link points to
                Quest next = node.Quest.Links[i].NextQuest;
                if (next != null)
                {
                    // Create a connection. It's not hard
                    CreateConnection(node.outPoints[i], GetNodeByQuest(next).inPoint);
                }
            }
        }
        // clean selections
        ClearConnectionSelection();
    }

    /// <summary>
    /// Create a List of DialogueLineNodes from an array of DialogueLines and adds them to the nodes List
    /// Previously Created DialogueLineNodes are overwritten
    /// </summary>
    /// <param name="lines">An array containing all the DialogueLines that need to be parsed</param>
    /// <returns>A List of DialogueLineNodes matching the DialogueLines</returns>
    private void ShowQuestDialogue(QuestNode questNode)
    {
        // Set State variables
        State = WindowState.DialogueView;
        currentQuestNode = questNode;

        // Reset Nodes, Connections and DialogueLineNodes
        nodes = new List<Node>();
        connections = new List<Connection>();
        dialogueLineNodes = new List<DialogueLineNode>();

        // Create DialogueLineNodes from DialogueLine Assets
        foreach (DialogueLine line in questNode.Quest.Lines) 
        {
            DialogueLineNode node = new DialogueLineNode(questNode, line, defaultNodeData);
            dialogueLineNodes.Add(node);
            nodes.Add((Node)node);
        }

        // Add the QuestNode with a connection to the first DialogueLineNode
        nodes.Add(questNode);
        questNode.displayedOutPoints = 2;
        if (questNode.Quest.PrecedingStartLine != null)
        {
            CreateConnection(questNode.outPointPreceding, GetNodeByDialogueLine(questNode.Quest.PrecedingStartLine).inPoint);
        }
        if (questNode.Quest.SucceedingStartLine != null)
        {
            CreateConnection(questNode.outPointSucceding, GetNodeByDialogueLine(questNode.Quest.SucceedingStartLine).inPoint);
        }

        // Assign node connections
        foreach (DialogueLineNode node in dialogueLineNodes)
        {
            DialogueLine line = node.Line;

            // Set connections to the left and right, if applicable
            if (line.NextRight != null)
            {
                CreateConnection(node.outPointRight, GetNodeByDialogueLine(line.NextRight).inPoint);
            }
            if (line.NextLeft != null)
            {
                CreateConnection(node.outPointLeft, GetNodeByDialogueLine(line.NextLeft).inPoint);
            }

            // Clear whatever selected in and out points remain
            ClearConnectionSelection();
        }
    }

    /// <summary>
    /// Return the view to the Quest View from the Dialogue View
    /// </summary>
    private void DialogueToQuestView()
    {
        currentQuestNode.displayedOutPoints = currentQuestNode.Quest.Links.Count;
        
        // Throw an error if the editor is not in dialogue view
        if (State != WindowState.DialogueView)
        {
            Debug.LogError("Tried to run DialogueToQuestView when not in DialogueView");
        }

        // as questNodes are not cleared when going to dialogueView, we can just do this
        nodes = questNodes.Select( node => (Node)node).ToList();

        // reset connections and currentQuestNode
        connections = new List<Connection>();
        currentQuestNode = null;
        // Set the window state
        State = WindowState.QuestView;

        // Draw the connections between the quests
        CreateQuestNodeConnections();
    }

    /// <summary>
    /// Get a DialogueLineNode from it's associated DialogueLine
    /// </summary>
    /// <param name="line">The nodes associated Dialogue Line</param>
    /// <returns>The DialogueLineNode</returns>
    private DialogueLineNode GetNodeByDialogueLine(DialogueLine line)
    {
        // Go through all nodes, if one matches, return it;
        return (DialogueLineNode)nodes
        .Where( e => ((DialogueLineNode)e).Line == line)
        .First();
    }

    /// <summary>
    /// Get a Quest Node from a Quest reference
    /// </summary>
    /// <param name="quest">A quest reference</param>
    /// <returns>The QuestNode with this quest</returns>
    private QuestNode GetNodeByQuest(Quest quest)
    {
        return questNodes
        .Where( e => e.Quest == quest )
        .First();
    }

    public void UpdateQuestNode(Quest quest)
    {
        GetNodeByQuest(quest)?.UpdateContent();
    }
    public void UpdateAllQuestNodes()
    {
        foreach (QuestNode node in questNodes)
        {
            node.UpdateContent();
        }
    }

    /// <summary>
    /// Create a new DialogueLine Asset and a Node for it
    /// </summary>
    /// <param name="position"> Vector2 of the position to create the Node at</param>
    private void CreateNewDialogueLine(Vector2 position)
    {
        // Get a unique path for a line
        string path = AssetDatabase.GenerateUniqueAssetPath($"{dialoguePath}/Line.asset");

        // Create the new asset
        DialogueLine line = ScriptableObject.CreateInstance<DialogueLine>();
        AssetDatabase.CreateAsset(line, path);

        // Add the Dialogue Line to the currently viewed Quest
        // If this line throws an error, you are trying to create a new dialogue line when not in dialogue view
        currentQuestNode.Quest.Lines.Add(line);
        // Mark the currently viewed quest to be saved
        EditorUtility.SetDirty(currentQuestNode.Quest);

        // Set the lines inital title
        line.Title = line.name;
        // The Lines position is always relative to the questNode
        line.EditorPos = position - currentQuestNode.Quest.EditorPos;
        // Mark the line to be saved
        EditorUtility.SetDirty(line);

        // Save changes
        AssetDatabase.SaveAssets();

        // Create a node for the line
        DialogueLineNode node = new DialogueLineNode(currentQuestNode, line, defaultNodeData);

        // Add node to lists
        nodes.Add(node);
        dialogueLineNodes.Add(node);
    }

    /// <summary>
    /// Create a new Quest Asset and a Node for it
    /// </summary>
    /// <param name="position"> Vector2 of the position to create the Node at</param>
    private void CreateNewQuest(Vector2 position)
    {
        // Get a unique path for for a quest
        string path = AssetDatabase.GenerateUniqueAssetPath($"{dialoguePath}/Quest.asset");

        // Create the new asset
        Quest quest = ScriptableObject.CreateInstance<Quest>();
        AssetDatabase.CreateAsset(quest, path);

        // set the quests initial title and position
        quest.Title = quest.name;
        quest.EditorPos = position;

        // Mark quest asset to be saved
        EditorUtility.SetDirty(quest);

        // Save changes to assets
        AssetDatabase.SaveAssets();

        // Create a node for the quest
        QuestNode node = new QuestNode(quest, defaultNodeData, ShowQuestDialogue, OnQuestNodeDragEnd);

        // add the node to lists
        nodes.Add(node);
        questNodes.Add(node);
    }

    /// <summary>
    /// OnGUI is called whenever the window is being drawn. This is essentially the main function of the Window
    /// </summary>
    private void OnGUI()
    {
        // Draw two grids for orientation
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        GUI.Label(new Rect(0, 0, 100, 100), State.ToString(), new GUIStyle());

        // Draw all connections on the screen
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].Draw();
        }
        
        // Draw all nodes on the screen
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Draw();
        }


        // Draw a line from mouse to SelectedInPoint if the user still has to select an outPoint
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                Event.current.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                Event.current.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true; // Set this to redraw
        }

        // Draw a line from mouse to SelectedOutPoint if the user still has to select an inPoint
        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                Event.current.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                Event.current.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true; // Set this to redraw
        }

        // Process Events for all nodes
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            bool guiChanged = nodes[i].ProcessEvents(Event.current);

            if (guiChanged)
            {
                GUI.changed = true;
            }
        }

        // Reset drag vector
        drag = Vector2.zero;

        // Process Event for Window
        switch (Event.current.type)
        {
            // Mouseclick Events
            case EventType.MouseDown:
                // On LeftClick clear selected connection points
                if (Event.current.button == 0)
                {
                    ClearConnectionSelection();
                }   
                // On RightClick open the context menu
                if (Event.current.button == 1)
                {
                    // Create a generic menu at the mouse position and add options depending on the current view
                    GenericMenu genericMenu = new GenericMenu();
                    Vector2 mousePosition = Event.current.mousePosition;
                    switch (State)
                    {
                        case WindowState.DialogueView:
                            genericMenu.AddItem(new GUIContent("Add Dialogue Line"), false, () => CreateNewDialogueLine(mousePosition));
                            genericMenu.AddItem(new GUIContent("Return to Quest View"), false, DialogueToQuestView);
                            break;
                        case WindowState.QuestView:
                            genericMenu.AddItem(new GUIContent("Add Quest"), false, () => CreateNewQuest(mousePosition));
                            break;
                    }
                    genericMenu.ShowAsContext();
                }
                break;

            // Mousedrag Events
            case EventType.MouseDrag:
                // If Left Mouse Button is clicked, drag
                if (Event.current.button == 0)
                {
                    isDragging = true;
                    // Set the windows drag vector. This gets added to the windows offset, saving
                    drag = Event.current.delta;

                    // Drag all nodes along with the window
                    nodes.ForEach( e => e.Drag(drag));
                    questNodes.Except(nodes).ToList().ForEach( e => e.Drag(drag));

                    // Log window changes so we get a repaint
                    GUI.changed = true;
                }
                break;

            // On Mouse Up
            case EventType.MouseUp:
                if (isDragging)
                {
                    // i constantly forgt this so i have to write this in bold:
                    // THIS ONLY FIRES WHEN DRAGGING THE SCREEN 
                    foreach (QuestNode node in questNodes)
                    {
                        node.Quest.EditorPos = node.rect.position;
                        EditorUtility.SetDirty(node.Quest);
                    }
                    AssetDatabase.SaveAssets();
                    isDragging = false;
                }
                break;

            case EventType.ScrollWheel:
                foreach(Node node in nodes) {
                    Vector2 newPos = node.rect.position;
                    newPos -= Event.current.mousePosition;
                    newPos *= 1f - 0.1f * Mathf.Sign(Event.current.delta.y);
                    newPos += Event.current.mousePosition;
                    node.rect.position = newPos;
                }
                break;
        }

        // If the window changed in any way, redraw it.
        if (GUI.changed) Repaint();
    }

    /// <summary>
    /// Draw a grid in the window
    /// </summary>
    /// <param name="gridSpacing">The spacing between the grids line</param>
    /// <param name="gridOpacity">The opacity of the grids lines</param>
    /// <param name="gridColor">The color of the girds lines</param>
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        // calculate how many lines to draw
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        // Begin drawing
        Handles.BeginGUI();
        // Set color
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        // move the offset by the drag amount
        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        // Draw Lines
        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        // Reset color
        Handles.color = Color.white;
        // Stop Drawing
        Handles.EndGUI();
    }

    /// <summary>
    /// Callback for clicking on an inPoint
    /// </summary>
    /// <param name="inPoint">the ConnectionPoint clicked</param>
    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        // note this point
        selectedInPoint = inPoint;

        // If there already is a selected out point, create a connection
        if (selectedOutPoint != null)
        {
            // if the out point already has a connection, remove it
            if (selectedOutPoint.connection != null) {
                RemoveConnection(selectedOutPoint.connection);
            }

            // If the nodes are not the same, create the connection
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                connections.Add(Connection.SetNewConnection(selectedInPoint, selectedOutPoint, RemoveConnection));
            }

            // cleanup
            ClearConnectionSelection();
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        // if the out point already has a connection, remove it
        if (outPoint.connection != null) {
            RemoveConnection(outPoint.connection);
        }

        // Note this point
        selectedOutPoint = outPoint;

        // If there already is a selected in point, create the connection
        if (selectedInPoint != null)
        {
            // If the nodes are not the same, create the point
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                connections.Add(Connection.SetNewConnection(selectedInPoint, selectedOutPoint, RemoveConnection));
            }

            // cleanup
            ClearConnectionSelection();
        }
    }

    /// <summary>
    /// This is passed to the quest node to be executed when a drag ends. It is used to modify editor positions of different elements throughout diefferent views.
    /// </summary>
    /// <param name="node">Reference to the node whose drag event just ended</param>
    private void OnQuestNodeDragEnd(QuestNode node)
    {
        // Calculate the vector from the objects original position to the new one
        Vector2 dragVector = node.rect.position - node.Quest.EditorPos;

        // When in DialogueView, special behaviour is necessary to preserve relative positions
        if (DialogueEditor.Instance.State == DialogueEditor.WindowState.DialogueView)
        {
            // Move the quest Nodes that are currently not being rendered by the same vector
            // This way, when moving the quest node, you are actually moving the whole quest tree, thereby preserving the relative position
            foreach (QuestNode questNode in questNodes.Where(e => e != node).ToList())
            {
                // Move the actual rectangle and the editor position in the asset
                questNode.rect.position += dragVector;
                questNode.Quest.EditorPos += dragVector;
                // Mark the asset as dirty so it will be saved properly
                EditorUtility.SetDirty(questNode.Quest);
            }
            // Modify the dialogue lines editor positions by the inverse drag vector to preserve the relative position
            foreach (DialogueLineNode dialogueLineNode in nodes.Where(e => e is DialogueLineNode).Select(e => e as DialogueLineNode))
            {
                dialogueLineNode.Line.EditorPos -= dragVector;
                // Mark the asset as dirty
                EditorUtility.SetDirty(dialogueLineNode.Line);
            }
        }
        
        // Save the new Editor Position of the dragged quest node
        node.Quest.EditorPos = node.rect.position;
        // Mark the asset as dirty
        EditorUtility.SetDirty(node.Quest);
        // Save assets
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Remove a node
    /// </summary>
    /// <param name="node">The node to be removed</param>
    private void RemoveNode(Node node)
    {

        // Go through connections and check if they include the removed node
        List<Connection> connectionsToRemove = new List<Connection>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].inPoint.node == node || connections[i].outPoint.node == node)
            {
                connectionsToRemove.Add(connections[i]);
            }
        }
        for (int i = 0; i < connectionsToRemove.Count; i++)
        {
            RemoveConnection(connectionsToRemove[i]);
        }

        // Remove associated dialogue line assets
        if (node is DialogueLineNode)
        {
            // Get the dialogue line
            DialogueLine line = (node as DialogueLineNode).Line;
            // remove the dialogueline from the currentQuestNode
            // If this line throws an error, you are probably trying to remove a dialoguenode while not in DialogueView
            currentQuestNode.Quest.Lines.Remove(line);
            dialogueLineNodes.Remove((DialogueLineNode)node);
            // Remove the asset
            AssetDatabase.DeleteAsset($"{dialoguePath}/{line.name}.asset");
            // Save changes
            AssetDatabase.SaveAssets();
        }

        // Remove associated quest assets
        if (node is QuestNode)
        {
            // Get a reference to the quest asset
            Quest quest = (node as QuestNode).Quest;

            // If in dialogue view, go back to quest view, as viewing the dialogue of a non-existant quest node would not make sense
            if (State == WindowState.DialogueView)
            {
                DialogueToQuestView();
            }
            questNodes.Remove((QuestNode)node);
            // Remove the asset
            AssetDatabase.DeleteAsset($"{dialoguePath}/{quest.name}.asset");
            // Remove all associated lines assets
            foreach (DialogueLine line in quest.Lines)
            {
                AssetDatabase.DeleteAsset($"{dialoguePath}/{line.name}.asset");
            }
            // Save changes
            AssetDatabase.SaveAssets();
        }

        // Finally remove the node
        nodes.Remove(node);
    }
    /// <summary>
    /// Remove the Node representation of a dialogueLine
    /// </summary>
    /// <param name="line">Reference to the dialogue line asset</param>
    public void RemoveNode(DialogueLine line)
    {
        RemoveNode(GetNodeByDialogueLine(line));
    }
    /// <summary>
    /// Remove the Node representation of a quest
    /// </summary>
    /// <param name="line">Reference to the quest asset</param>
    public void RemoveNode(Quest quest)
    {
        RemoveNode(GetNodeByQuest(quest));
    }

    /// <summary>
    /// Remove the connection between two nodes
    /// </summary>
    /// <param name="connection"></param>
    private void RemoveConnection(Connection connection)
    {
        connection.Remove();
        connections.Remove(connection);
    }

    /// <summary>
    /// Create a connection between an inPoint and an outPoint
    /// </summary>
    /// <param name="outPoint">The outPoint</param>
    /// <param name="inPoint">The inPoint</param>
    private void CreateConnection(ConnectionPoint outPoint, ConnectionPoint inPoint)
    {
        // Create the connection
        Connection connection = new Connection(inPoint, outPoint, RemoveConnection);

        // Reference connection in the connection points
        inPoint.connection = connection;
        outPoint.connection = connection;

        // Finally add connection to list
        connections.Add(connection);
    }

    /// <summary>
    /// Clear the selected connection points
    /// </summary>//  
    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}