using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SetVariableNode : DialogueNode
{
    public override DialogueLine NextLine
    {
        get
        {
            VariablesManager.SetVariable(Key, Value);
            return NextNode?.NextLine;
        }
    }
    [HideInInspector] public DialogueNode NextNode;
    public string Key, Value;

#if UNITY_EDITOR
    public ConnectionPoint OutPoint;
    public override void OnEnable()
    {
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick);
        base.OnEnable();
        
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png") as Texture2D;

        if (Key == null && AllVariables.Count() > 0) Key = AllVariables.First();
    }

    public override void Draw(Vector2 offset, int state = 0)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        InPoint.Draw();
        OutPoint.Draw();
        base.Draw(offset, state);
        GUI.Label(rect, Title, LabelStyle);
    }

    public override void ProcessEvent(Event e, int state = 0, List<StoryNode> relatedNodes = null)
    {
        InPoint.ProcessEvent(e);
        OutPoint.ProcessEvent(e);
        base.ProcessEvent(e, state, relatedNodes);
    }

    public override List<Connection> GetOutConnections(int state)
    {
        List<Connection> connections = new List<Connection>();

        if (NextNode != null)
            connections.Add(new Connection(NextNode.InPoint.Center, OutPoint.Center, () => NextNode = null));

        return connections;
    }

    public override void OnOutPointClick(int index)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null)
        {
            NextNode = inPoint.Parent as DialogueNode;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
            ConnectionPoint.selectedOutPoint = OutPoint;
    }
    private static IEnumerable<SetVariableNode> AllVariableNodes =>
        AssetDatabase.FindAssets("t:SetVariableNode")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (SetVariableNode)AssetDatabase.LoadAssetAtPath(e, typeof(SetVariableNode)));

    public static IEnumerable<string> AllVariables =>
        AllVariableNodes
        .Select(e => e.Key);

    public static IEnumerable<string> GetAllValuesForVariable(string Key) =>
        AllVariableNodes
        .Where(e => e.Key == Key)
        .Select(e => e.Value);

#endif
}