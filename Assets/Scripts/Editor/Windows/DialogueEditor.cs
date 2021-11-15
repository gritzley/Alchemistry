using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueEditor : EditorWindow
{
    // Nodes and connections
    [SerializeField] private List<Node> nodes;
    [SerializeField] private List<Connection> connections;
    [SerializeField] private List<ConnectionPoint> connectionPoints;

    // Style
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;


    // Refs selected points
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    // offset and drag are used to log the current position in the grid
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 drag;


    // Show the window
    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow()
    {
        DialogueEditor window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    private void OnEnable()
    {
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

        // Fill node list
        DialogueLine firstLine = AssetDatabase.LoadMainAssetAtPath("Assets/Objects/Line 1.asset") as DialogueLine;

        GetDialogueTreeFromLine(firstLine);

        // var data = EditorPrefs.GetString("DialogueEditorContent", JsonUtility.ToJson(this, false));

        // // Then we apply them to this window
        // JsonUtility.FromJsonOverwrite(data, this);
    }
    
    private void OnDisable()
    {

        // var data = JsonUtility.ToJson(this, false);
        // // And we save it
        // EditorPrefs.SetString("DialogueEditorContent", data);

    }

    public void GetDialogueTreeFromLine(DialogueLine startLine)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        Vector2 currentPos = new Vector2(0, 0);

        Dictionary<DialogueLine, DialogueLineNode> nodeLookupTable = new Dictionary<DialogueLine, DialogueLineNode>();
        List<List<DialogueLine>> LayeredDialogueLines = new List<List<DialogueLine>>();
        LayeredDialogueLines.Add(new List<DialogueLine>() {startLine});
        DialogueLineNode node = new DialogueLineNode(startLine, currentPos, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        nodeLookupTable.Add(startLine, node);
        nodes.Add(node);



        for (int i = 0; i < LayeredDialogueLines.Count; i++)
        {
            List<DialogueLine> layer = LayeredDialogueLines[i];

            currentPos.y = (layer.Count - 1) * -50;

            currentPos.x += 150;
            for (int j = 0; j < layer.Count; j++)
            {
                DialogueLine line = layer[j];

                if (line.NextLeft != null || line.NextRight != null )
                {
                    if (i == LayeredDialogueLines.Count - 1) {
                        LayeredDialogueLines.Add(new List<DialogueLine>());
                    }
                    if (line.NextRight != null)
                    {
                        if (!nodeLookupTable.ContainsKey(line.NextRight))
                        {
                            node = new DialogueLineNode(line.NextRight, currentPos, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                            nodeLookupTable.Add(line.NextRight, node);
                            nodes.Add(node);
                            LayeredDialogueLines[i+1].Add(line.NextRight);
                            currentPos.y += 100;
                        }
                        else
                        {
                            node = nodeLookupTable[line.NextRight];
                        }
                        selectedOutPoint = nodeLookupTable[line].outPointRight;
                        selectedInPoint = node.inPoint;
                        CreateConnection();
                    }
                    if (line.NextLeft != null)
                    {
                        if (!nodeLookupTable.ContainsKey(line.NextLeft))
                        {
                            node = new DialogueLineNode(line.NextLeft, currentPos, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                            nodeLookupTable.Add(line.NextLeft, node);
                            nodes.Add(node);
                            LayeredDialogueLines[i+1].Add(line.NextLeft);
                            currentPos.y += 100;
                        }
                        else
                        {
                            node = nodeLookupTable[line.NextLeft];
                        }
                        selectedOutPoint = nodeLookupTable[line].outPointLeft;
                        selectedInPoint = node.inPoint;
                        CreateConnection();
                    }
                }
                ClearConnectionSelection();
            }
        }
    }

    private void OnGUI()
    {
        // Draw a cute grid
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        // Draw content
        DrawNodes();
        DrawConnections();

        // Draw the line for the current drag event
        DrawConnectionLine(Event.current);

        // Process window events (clicks and drags and stuff)
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        // If the window changed in any way, redraw it.
        if (GUI.changed) Repaint();
    }

    // Draws a grid
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

    // Draw all the nodes
    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    // Draw all the connections
    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            } 
        }
    }

    // Process window events
    private void ProcessEvents(Event e)
    {
        // Set the Drag to 0
        drag = Vector2.zero;

        switch (e.type)
        {
            // On Mousedown
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    // On leftclick clear selection
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    // On rightlick open context menu
                    ProcessContextMenu(e.mousePosition);
                }
            break;

            // On drag set the drag ref
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
            break;
        }
    }

    // Process events for all nodes
    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    // Draw connection line to the mouse on setting connections
    private void DrawConnectionLine(Event e)
    {
        // When dragging from in to out
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true; // Set this to redraw
        }

        // When dragging from out to in
        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true; // Set this to redraw
        }
    }

    // Display a context menu
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        // Just a point to add nodes
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition)); 
        genericMenu.ShowAsContext();
    }

    // when dragging do this
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        // Go through all nodes and call their drags
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    // Add a new node at a mouse position
    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(mousePosition, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    // When clicking an  in connection point
    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        // note this point
        selectedInPoint = inPoint;

        // if there is a selected out point, set a connection, unless it is the same node
        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection(); 
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        // Note this point
        selectedOutPoint = outPoint;

        // if there is a selected in point, set a connection, unless it is the same node
        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
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
                connections.Remove(connectionsToRemove[i]);
            }

            // cleanup
            connectionsToRemove = null;
        }

        // Finally remove the node
        nodes.Remove(node);
    }

    // Remove a connection
    private void OnClickRemoveConnection(Connection connection)
    {
        DialogueLineNode node = connection.outPoint.node as DialogueLineNode;

        if (connection.outPoint == node.outPointLeft )
        {
            node.Line.NextLeft = null;
        }
        else if (connection.outPoint == node.outPointRight)
        {
            node.Line.NextRight = null;
        }
        
        connections.Remove(connection);
    }

    // Create a connection
    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        DialogueLineNode node = selectedOutPoint.node as DialogueLineNode;

        if (selectedOutPoint == node.outPointLeft )
        {
            node.Line.NextLeft = (selectedInPoint.node as DialogueLineNode).Line;
        }
        else if (selectedOutPoint == node.outPointRight)
        {
            node.Line.NextRight = (selectedInPoint.node as DialogueLineNode).Line;
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    // Clear the selected connection points
    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}