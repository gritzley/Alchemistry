using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [HideInInspector] public Quaternion Rotation;
    void OnEnable()
    {
        Rotation = transform.rotation;
    }
    public WaitForSeconds PickUp(Transform newParent)
    {
        transform.parent = newParent;
        transform.LeanMoveLocal(Vector3.zero, 0.15f);
        return new WaitForSeconds(0.15f);
    }
    public void OnMouseDown()
    {
        if (PlayerController.Instance.HeldItem == null)
        {
            PickUp(PlayerController.Instance.HandTransform);
            PlayerController.Instance.HeldItem = this;
        }
    }
}
