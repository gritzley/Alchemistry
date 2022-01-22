using System;
using System.Collections.Generic;
using UnityEngine;

public class NewspaperArticle : SceneNode
{
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
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Size.y = 40;
    }
}