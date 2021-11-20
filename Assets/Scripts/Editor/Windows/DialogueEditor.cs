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

        // Set all the styles
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        // Initialize default node data
        defaultNodeData = new NodeData();
        defaultNodeData.nodeStyle = nodeStyle;
        defaultNodeData.selectedNodeStyle = selectedNodeStyle;
        defaultNodeData.inPointStyle = inPointStyle;
        defaultNodeData.outPointStyle = outPointStyle;
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

    private void ShowQuests(Quest[] quests)
    {
        // Set Window State
        State = WindowState.QuestView;

        // Reset Nodes, Connections and QuestNodes
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

        DrawQuestViewConnections();
    }

    private void DrawQuestViewConnections()
    {
        foreach (QuestNode node in questNodes)
        {
            for (int i = 0; i < node.Quest.Links.Count; i++)
            {
                Quest next = node.Quest.Links[i].NextQuest;
                if (next != null)
                {
                    selectedInPoint = GetNodeByQuest(next).inPoint;
                    selectedOutPoint = node.linkOutPoints[i];
                    CreateConnection();
                }
            }
        }
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
        if (questNode.Quest.StartLine != null)
        {
            selectedOutPoint = questNode.outPoint;
            selectedInPoint = GetNodeByDialogueLine(questNode.Quest.StartLine).inPoint;
            CreateConnection();
            ClearConnectionSelection();
        }

        // Assign node connections
        foreach (DialogueLineNode node in dialogueLineNodes)
        {
            DialogueLine line = node.Line;

            // Set connections to the left and right, if applicable
            if (line.NextRight != null)
            {
                DialogueLineNode nextNode = GetNodeByDialogueLine(line.NextRight);
                selectedOutPoint = node.outPointRight;
                selectedInPoint = nextNode.inPoint;
                CreateConnection();
            }
            if (line.NextLeft != null)
            {
                DialogueLineNode nextNode = GetNodeByDialogueLine(line.NextLeft);
                selectedOutPoint = node.outPointLeft;
                selectedInPoint = nextNode.inPoint;
                CreateConnection();
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
        if (State != WindowState.DialogueView)
        {
            Debug.LogError("Tried to run DialogueToQuestView when not in DialogueView");
        }
        nodes = questNodes.Select( node => (Node)node).ToList();
        connections = new List<Connection>();
        dialogueLineNodes = new List<DialogueLineNode>();
        State = WindowState.QuestView;
        currentQuestNode = null;

        DrawQuestViewConnections();
    }

    /// <summary>
    /// Get a DialogueLineNode from it's associated DialogueLine
    /// </summary>
    /// <param name="line">The nodes associated Dialogue Line</param>
    /// <returns>The DialogueLineNode</returns>
    private DialogueLineNode GetNodeByDialogueLine(DialogueLine line)
    {
        // Go through all nodes, if one matches, return it;
        foreach (DialogueLineNode node in dialogueLineNodes)
        {
            if (node.Line == line)
            {
                return node;
            }
        }

        // If all fails return null
        return null;
    }

    private QuestNode GetNodeByQuest(Quest quest)
    {
        foreach (QuestNode node in questNodes)
        {
            if (node.Quest == quest)
            {
                return node;
            }
        }

        return null;
    }

    /// <summary>
    /// Called when the Editor Window is disabled
    /// </summary>
    private void OnDisable()
    {
        // Deinstantiate
        Instance = null;
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

        // Draw all nodes on the screen
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Draw();
        }

        // Draw all connections on the screen
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].Draw();
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
                    GenericMenu genericMenu = new GenericMenu();
                    Vector2 mousePosition = Event.current.mousePosition;
                    // Context Menu is relative to current window state
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
                    // Set the windows drag vector. This gets added to the windows offset, saving
                    drag = Event.current.delta;

                    // Drag all nodes along with the window
                    nodes.ForEach( e => e.Drag(drag));
                    questNodes.Except(nodes).ToList().ForEach( e => e.Drag(drag));

                    // Log window changes so we get a repaint
                    GUI.changed = true;
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
    /// Create a new DialogueLine Asset and a Node for it
    /// </summary>
    /// <param name="position"> Vector2 of the position to create the Node at</param>
    private void CreateNewDialogueLine(Vector2 position)
    {
        string path = AssetDatabase.GenerateUniqueAssetPath($"{dialoguePath}/Line.asset");

        DialogueLine line = ScriptableObject.CreateInstance<DialogueLine>();

        AssetDatabase.CreateAsset(line, path);
        AssetDatabase.SaveAssets();

        line.Title = line.name;
        line.EditorPos = position - currentQuestNode.Quest.EditorPos;

        currentQuestNode.Quest.Lines.Add(line);

        DialogueLineNode node = new DialogueLineNode(currentQuestNode, line, defaultNodeData);

        nodes.Add(node);
        dialogueLineNodes.Add(node);
    }

    /// <summary>
    /// Create a new Quest Asset and a Node for it
    /// </summary>
    /// <param name="position"> Vector2 of the position to create the Node at</param>
    private void CreateNewQuest(Vector2 position)
    {
        string path = AssetDatabase.GenerateUniqueAssetPath($"{dialoguePath}/Quest.asset");

        Quest quest = ScriptableObject.CreateInstance<Quest>();

        AssetDatabase.CreateAsset(quest, path);
        AssetDatabase.SaveAssets();

        quest.Title = quest.name;
        quest.EditorPos = position;

        QuestNode node = new QuestNode(quest, defaultNodeData, ShowQuestDialogue, OnQuestNodeDragEnd);

        nodes.Add(node);
        questNodes.Add(node);
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
                CreateConnection();
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
                CreateConnection();
            }

            // cleanup
            ClearConnectionSelection();
        }
    }

    private void OnQuestNodeDragEnd(QuestNode node)
    {
        Vector2 dragVector = node.rect.position - node.Quest.EditorPos;
        if (DialogueEditor.Instance.State == DialogueEditor.WindowState.DialogueView)
        {
            foreach (QuestNode questNode in questNodes.Where(e => e != node).ToList())
            {
                questNode.rect.position += dragVector;
                questNode.Quest.EditorPos += dragVector;
            }
            foreach (DialogueLineNode dialogueLineNode in dialogueLineNodes)
            {
                dialogueLineNode.Line.EditorPos -= dragVector;
            }
        }
        node.Quest.EditorPos = node.rect.position;
    }

    // Remove a node
    private void RemoveNode(Node node)
    {
        // If there are connections, clear the ones connected to this one
        if (connections != null)
        {
            // Go through connections and check if they include the removed node
            List<Connection> connectionsToRemove = new List<Connection>();
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            // remove the nodes you found
            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                RemoveConnection(connectionsToRemove[i]);
            }

            // cleanup
            connectionsToRemove = null;
        }

        if (node is DialogueLineNode)
        {
            DialogueLine line = (node as DialogueLineNode).Line;
            currentQuestNode.Quest.Lines.Remove(line);
            AssetDatabase.DeleteAsset($"{dialoguePath}/{line.name}.asset");
            AssetDatabase.SaveAssets();
        }
        if (node is QuestNode)
        {
            Quest quest = (node as QuestNode).Quest;
            if (State == WindowState.DialogueView)
            {
                DialogueToQuestView();
            }
            AssetDatabase.DeleteAsset($"{dialoguePath}/{quest.name}.asset");
            foreach (DialogueLine line in quest.Lines)
            {
                AssetDatabase.DeleteAsset($"{dialoguePath}/{line.name}.asset");
            }
            AssetDatabase.SaveAssets();
        }

        // Finally remove the node
        nodes.Remove(node);
    }
    public void RemoveNode(DialogueLine line)
    {
        RemoveNode(GetNodeByDialogueLine(line));
    }
    public void RemoveNode(Quest quest)
    {
        RemoveNode(GetNodeByQuest(quest));
    }

    // Remove a connection
    private void RemoveConnection(Connection connection)
    {

        // get outNode
        Node outNode = connection.outPoint.node;
        Node inNode = connection.inPoint.node;

        // If outNode is a DialogueLine set NextLeft or NextRight to null, depending on where the connection is
        if (outNode is DialogueLineNode)
        {
            DialogueLineNode lineNode = (outNode as DialogueLineNode);
            if (connection.outPoint == lineNode.outPointLeft )
            {
                lineNode.Line.NextLeft = null;
            }
            else if (connection.outPoint == lineNode.outPointRight)
            {
                lineNode.Line.NextRight = null;
            }
        }

        if ((outNode is QuestNode) && (inNode is DialogueLineNode))
        {
            (outNode as QuestNode).Quest.StartLine = null;
        }

        if ((outNode is QuestNode) && (inNode is QuestNode))
        {
            Quest quest = (outNode as QuestNode).Quest;
            int index = (outNode as QuestNode).linkOutPoints.IndexOf(connection.outPoint);
            
            Quest.Link link = quest.Links[index];
            link.NextQuest = null;
            quest.Links[index] = link;            
        }

        // remove connectionpoints references to connection;
        connection.inPoint.connection = null;
        connection.outPoint.connection = null;
        
        // Finally remove the conenction
        connections.Remove(connection);
    }

    // Create a connection
    private void CreateConnection()
    {
        // get in and out node
        Node _outNode = selectedOutPoint.node;
        Node _inNode = selectedInPoint.node;



        // DialogueLine -> DialogueLine
        if ((_outNode is DialogueLineNode) && (_inNode is DialogueLineNode))
        {
            DialogueLineNode outNode = (DialogueLineNode)_outNode;
            DialogueLineNode inNode = (DialogueLineNode)_inNode;

            // Select whether to put the line in nextLeft or nextRight
            if (selectedOutPoint == outNode.outPointLeft )
            {
                outNode.Line.NextLeft = (selectedInPoint.node as DialogueLineNode).Line;
            }
            else if (selectedOutPoint == outNode.outPointRight)
            {
                outNode.Line.NextRight = (selectedInPoint.node as DialogueLineNode).Line;
            }
        }

        // Quest -> DialogueLine
        if ((_outNode is QuestNode) && (_inNode is DialogueLineNode))
        {
            (_outNode as QuestNode).Quest.StartLine = (_inNode as DialogueLineNode).Line;
        }

        // Quest -> Quest
        if ((_outNode is QuestNode) && (_inNode is QuestNode))
        {
            QuestNode outNode = (QuestNode)_outNode;
            int index = outNode.linkOutPoints.IndexOf(selectedOutPoint);
            if (index >= 0)
            {
                Quest.Link link = outNode.Quest.Links[index];
                link.NextQuest = (_inNode as QuestNode).Quest;
                outNode.Quest.Links[index] = link;
            }
        }



        // Create connection
        Connection connection = new Connection(selectedInPoint, selectedOutPoint, RemoveConnection);

        // Reference connection in the connection points
        selectedInPoint.connection = connection;
        selectedOutPoint.connection = connection;

        // Finally add connection to list
        connections.Add(connection);
    }

    // Clear the selected connection points
    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}