using System;
using UnityEngine;

public class BookPageCollider : Interactible
{
    public Action OnClick;

    public override bool OnInteract(PlayerController player)
    {
        if (OnClick != null) OnClick.Invoke();
        return true;
    }
}
