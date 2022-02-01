using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pickupable : Clickable
{
    public WaitForSeconds PickUp(Transform newParent, float seconds = 0.15f)
    {
        if (newParent == PlayerController.Instance.HandTransform)
            SetLayerRecursively(gameObject, 2);
        else
            SetLayerRecursively(gameObject, 0);

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

    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
    
        foreach(Transform child in obj.GetComponentsInChildren<Transform>().Where(e => e.gameObject != obj) )
            SetLayerRecursively(child.gameObject, layer);
    }

}
