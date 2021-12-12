using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class StoryEditor : EditorWindow
{
    // The amount of offset the Window has
    Vector2 offset;

    // Flag to see whether the view is currently being dragged
    bool isDragging;
    List<StoryNode> nodes;

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
        // Set the last logged position
        offset = new Vector2(PlayerPrefs.GetFloat("StoryEditorOffsetX", 0), PlayerPrefs.GetFloat("StoryEditorOffsetY", 0));

        // Collect Quest Assets
        nodes = AssetDatabase.FindAssets("t:Quest", new string[] { "Assets/Dialogue" })
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (StoryNode)AssetDatabase.LoadAssetAtPath(e, typeof(Quest)))
        .ToList();
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
        nodes.ForEach( e => e.ProcessEvent(Event.current));
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                ConnectionPoint.selectedInPoint = null;
                ConnectionPoint.selectedOutPoint = null;
                nodes.ForEach( e => e.isSelected = false);
                break;

            case EventType.MouseDrag:

                // ---- DRAG ----
                if (Event.current.button == 0)
                {
                    // Set flag
                    isDragging = true;

                    // Move the position in the view
                    Vector2 drag = Event.current.delta;
                    offset += drag;

                    // Log window changes so we get a repaint
                    GUI.changed = true;
                }
                break;

            case EventType.MouseUp:

                // ---- DRAG END ----
                if (isDragging)
                {
                    // Set flag
                    isDragging = false;

                    // Save Editor Position
                    PlayerPrefs.SetFloat("StoryEditorOffsetX", offset.x);
                    PlayerPrefs.SetFloat("StoryEditorOffsetY", offset.y);
                }
                break;
        }

        // ---- DRAW NODES & CONNECTIONS ----
        nodes.ForEach( e => e.Connections.ForEach( e => e.Draw()));
        nodes.ForEach( e => e.Draw(offset) );

        if (ConnectionPoint.selectedInPoint != null && ConnectionPoint.selectedOutPoint == null)
        {
            new Connection(ConnectionPoint.selectedInPoint.Center, Event.current.mousePosition).Draw();
            GUI.changed = true; // Set this to redraw
        }
        if (ConnectionPoint.selectedOutPoint != null && ConnectionPoint.selectedInPoint == null)
        {
            new Connection(Event.current.mousePosition, ConnectionPoint.selectedOutPoint.Center).Draw();
            GUI.changed = true; // Set this to redraw
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
    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        // calculate how many lines to draw
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