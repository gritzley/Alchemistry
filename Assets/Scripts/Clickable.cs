using UnityEngine;
public abstract class Clickable : MonoBehaviour
{
    public bool IsClickable = true;
    public abstract void OnClick();
    void OnMouseDown()
    {
        if (IsClickable) OnClick();
    }
}