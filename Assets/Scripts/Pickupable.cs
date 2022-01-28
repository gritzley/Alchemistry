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
    public WaitForSeconds PickUp(Transform newParent, float seconds = 0.15f)
    {
        transform.parent = newParent;
        transform.LeanMoveLocal(Vector3.zero, seconds);
        transform.LeanRotate(newParent.transform.rotation.eulerAngles, seconds);
        return new WaitForSeconds(seconds);
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
