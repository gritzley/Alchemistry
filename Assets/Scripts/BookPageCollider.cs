using System;
using UnityEngine;

public class BookPageCollider : MonoBehaviour
{
    public Action OnClick;
    public void OnMouseDown() => OnClick?.Invoke();
}
