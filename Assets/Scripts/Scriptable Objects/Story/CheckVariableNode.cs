using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CheckVariableNode : DialogueNode
{
    public override DialogueLine NextLine
    {
        get
        {
            string value = VariablesManager.GetVariable(Key);
            if (value != null)
                return ValueToNode[value].NextLine;
            else
                return null;
        }
    }
    public string Key;
    public List<string> Values;

    // Lotta Hoops to jump through if you want to serialize a dictionary in unity
    [Serializable] public class StringToNode : SerializableDictionary<string, DialogueNode> {}
    [SerializeField] private StringToNode _valueToNode;
    public StringToNode ValueToNode
    {
        get
        {
            if (_valueToNode == null) _valueToNode = new StringToNode();
            return _valueToNode;
        }
    }

#if UNITY_EDITOR
    public List<ConnectionPoint> OutPoints;
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateOutPoints();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;

        if (Key == null && SetVariableNode.AllVariables.Count() > 0) Key = SetVariableNode.AllVariables.First();

    }

    public void UpdateOutPoints()
    {
        Values = SetVariableNode.GetAllValuesForVariable(Key).ToList();
        OutPoints = new List<ConnectionPoint>();

        for (int i = 0; i < Values.Count; i++)
            OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, OnOutPointClick, i));

        Size.y = Mathf.Max(15 + 25 * Values.Count, 40);

        GUI.changed = true;
    }

    public override void Draw(Vector2 offset, int state = 0)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        InPoint.Draw();
        OutPoints.ForEach(e => e.Draw());
        base.Draw(offset, state);
        for (int i = 0; i < Values.Count; i++)
        {
            Rect labelRect = rect;
            labelRect.position += new Vector2(0, 25 * i);
            GUI.Label(labelRect, Values[i], LabelStyle);
        }
    }

    public override void ProcessEvent(Event e, int state = 0, List<StoryNode> relatedNodes = null)
    {
        InPoint.ProcessEvent(e);
        OutPoints.ForEach(p => p.ProcessEvent(e));
        base.ProcessEvent(e, state, relatedNodes);
    }

    public override List<Connection> GetOutConnections(int state)
    {
        List<Connection> connections = new List<Connection>();
        for (int i = 0; i < Values.Count; i++)
            if (ValueToNode.ContainsKey(Values[i]) && ValueToNode[Values[i]] != null)
                connections.Add(new Connection(ValueToNode[Values[i]].InPoint.Center, OutPoints[i].Center, () => ValueToNode[Values[i]] = null));

        return connections;
    }
    public override void OnOutPointClick(int index)
    {
        ConnectionPoint inPoint = ConnectionPoint.selectedInPoint;

        if (inPoint != null)
        {
            if (ValueToNode.ContainsKey(Values[index]))
                ValueToNode[Values[index]] = inPoint.Parent as DialogueNode;
            else
                ValueToNode.Add(Values[index], inPoint.Parent as DialogueNode);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            ConnectionPoint.selectedInPoint = null;
            ConnectionPoint.selectedOutPoint = null;
        }
        else
            ConnectionPoint.selectedOutPoint = OutPoints[index];
    }

#endif
}