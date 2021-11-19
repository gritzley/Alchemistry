using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class DialogueEditor : EditorWindow
{
    public static DialogueEditor Instance { get; private set; }
    public static bool IsOpen {
        get { return Instance != null; }
    }

    // Nodes and connections
    private List<Node> nodes;
    private List<Connection> connections;
    private List<DialogueLineNode> dialogueLineNodes;
    private List<QuestNode> questNodes;


    // Style
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    // default node data object
    private NodeData defaultNodeData;

    // Refs selected points
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    // offset and drag are used to log the current position in the grid
    private Vector2 offset;
    private Vector2 drag;

    // paths
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
        // Instantiate
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
        defaultNodeData.OnClickRemoveNode = OnClickRemoveNode;

        // Initialize lists
        nodes = new List<Node>();
        connections = new List<Connection>();
        dialogueLineNodes = new List<DialogueLineNode>();
        questNodes = new List<QuestNode>();
        
        // prepare the string array needed for the FindAssets method
        string[] directories = new string[] { dialoguePath };

        // Collect Line Assets
        DialogueLine[] lines = AssetDatabase.FindAssets("t:DialogueLine", directories)
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (DialogueLine)AssetDatabase.LoadAssetAtPath(e, typeof(DialogueLine)))
        .ToArray();

        // Collect Quest Assets (a generic function for this would be wicked)
        Quest[] quests = AssetDatabase.FindAssets("t:Quest", directories)
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Quest)AssetDatabase.LoadAssetAtPath(e, typeof(Quest)))
        .ToArray();

        // Creeate QuestNodes from Quest Assets
        foreach (Quest quest in quests)
        {
            QuestNode node = new QuestNode(quest, defaultNodeData);
            questNodes.Add(node);
            nodes.Add((Node)node);
        }

        CreateDialogueLineNodesFromArray(lines);
    }

    /// <summary>
    /// Create a List of DialogueLineNodes from an array of DialogueLines
    /// </summary>
    /// <param name="lines">An array containing all the DialogueLines that need to be parsed</param>
    /// <returns>A List of DialogueLineNodes matching the DialogueLines</returns>
    private void CreateDialogueLineNodesFromArray(DialogueLine[] lines)
    {
        nodes = nodes.Except(dialogueLineNodes).ToList();
        dialogueLineNodes = new List<DialogueLineNode>();
        // Create DialogueLineNodes from DialogueLine Assets
        foreach (DialogueLine line in lines) 
        {
            DialogueLineNode node = new DialogueLineNode(line, defaultNodeData);
            dialogueLineNodes.Add(node);
            nodes.Add((Node)node);
        }

        // Assign node connections
        foreach (DialogueLineNode node in dialogueLineNodes)
        {
            DialogueLine line = node.Line;

            // Set connections to the left and right, if applicable
            if (line.NextRight != null)
            {
                DialogueLineNode nextNode = GetNodeByLine(line.NextRight);
                selectedOutPoint = node.outPointRight;
                selectedInPoint = nextNode.inPoint;
                CreateConnection();
            }
            if (line.NextLeft != null)
            {
                DialogueLineNode nextNode = GetNodeByLine(line.NextLeft);
                selectedOutPoint = node.outPointLeft;
                selectedInPoint = nextNode.inPoint;
                CreateConnection();
            }

            // Clear whatever selected in and out points remain
            ClearConnectionSelection();
        }
    }

    /// <summary>
    /// Get a DialogueLineNode from it's associated DialogueLine
    /// </summary>
    /// <param name="line">The nodes associated Dialogue Line</param>
    /// <returns>The DialogueLineNode</returns>
    private DialogueLineNode GetNodeByLine(DialogueLine line)
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
                    genericMenu.AddItem(new GUIContent("Add Dialogue Line"), false, () => CreateNewDialogueLine(mousePosition));
                    genericMenu.AddItem(new GUIContent("Add Quest"), false, () => CreateNewQuest(mousePosition));
                    genericMenu.ShowAsContext();
                }
            break;

            // Mousedrag Events
            case EventType.MouseDrag:
                // Leftclick Drag
                if (Event.current.button == 0)
                {
                    // Get the drag vector
                    drag = Event.current.delta;

                    // Go through all nodes and call their drags
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        nodes[i].Drag(Event.current.delta);
                    }

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
        line.EditorPos = position;

        DialogueLineNode node = new DialogueLineNode(line, defaultNodeData);

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

        QuestNode node = new QuestNode(quest, defaultNodeData);

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

    // Remove a node
    private void OnClickRemoveNode(Node node)
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
            AssetDatabase.DeleteAsset($"Assets/Dialogue/{(node as DialogueLineNode).Line.name}.asset");
            AssetDatabase.SaveAssets();
        }

        // Finally remove the node
        nodes.Remove(node);
    }

    // Remove a connection
    private void RemoveConnection(Connection connection)
    {

        // get outNode
        Node _outNode = connection.outPoint.node;

        // If outNode is a DialogueLine set NextLeft or NextRight to null, depending on where the connection is
        if (_outNode is DialogueLineNode)
        {
            DialogueLineNode outNode = (_outNode as DialogueLineNode);
            if (connection.outPoint == outNode.outPointLeft )
            {
                outNode.Line.NextLeft = null;
            }
            else if (connection.outPoint == outNode.outPointRight)
            {
                outNode.Line.NextRight = null;
            }
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

        // if both nodes are DialogueNodes set in and out points appropriately
        if ((_outNode is DialogueLineNode) && (_inNode is DialogueLineNode))
        {
            DialogueLineNode outNode = (_outNode as DialogueLineNode);
            DialogueLineNode inNode = (_inNode as DialogueLineNode);

            if (selectedOutPoint == outNode.outPointLeft )
            {
                outNode.Line.NextLeft = (selectedInPoint.node as DialogueLineNode).Line;
            }
            else if (selectedOutPoint == outNode.outPointRight)
            {
                outNode.Line.NextRight = (selectedInPoint.node as DialogueLineNode).Line;
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