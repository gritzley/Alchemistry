using System.Collections;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    private Pickupable Item;
    private Collider Collider;
    void Start()
    {
        Item = GetComponentInChildren<Pickupable>();
        Collider = GetComponent<Collider>();
    }

    void OnMouseDown()
    {
        PlayerController player = PlayerController.Instance;
        if (PlayerController.Instance.HeldItem != null && Item == null)
        {
            player.HeldItem.PickUp(transform);
            Item = player.HeldItem;
            player.HeldItem = null;
        }
        else if (PlayerController.Instance.HeldItem == null && Item != null)
        {
            player.HeldItem = Item;
            player.HeldItem.PickUp(player.HandTransform);
            Item = null;
        }
    }
}