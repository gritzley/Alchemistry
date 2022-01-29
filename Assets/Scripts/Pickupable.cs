using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Clickable
{
    public WaitForSeconds PickUp(Transform newParent, float seconds = 0.15f)
    {
        transform.parent = newParent;
        transform.LeanMoveLocal(Vector3.zero, seconds);
        transform.LeanRotate(newParent.transform.rotation.eulerAngles, seconds);
        return new WaitForSeconds(seconds);
    }
    public override void OnClick()
    {
        if (PlayerController.Instance.HeldItem == null)
        {
            PickUp(PlayerController.Instance.HandTransform);
            PlayerController.Instance.HeldItem = this;
        }
    }
}
