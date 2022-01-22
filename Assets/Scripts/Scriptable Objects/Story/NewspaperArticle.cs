using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewspaperArticle : SceneNode
{
    public Sprite Sprite;

#if UNITY_EDITOR
    public override List<Connection> GetOutConnections(int state)
    {
        return new List<Connection>();
    }

    public override void OnOutPointClick(int index)
    {
        throw new NotImplementedException();
    }

    public override void Draw(Vector2 offset, int state = 0)
    {
        Size.x = LabelStyle.CalcSize(new GUIContent(Title)).x;
        InPoint.Draw();
        base.Draw(offset, state);
        GUI.Label(rect, Title, LabelStyle);

        if (Sprite == null) DrawNotification(2);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Size.y = 40;
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
    }

#endif

}