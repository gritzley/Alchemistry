using System.Collections;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    private Pickupable Item;
    private Collider Collider;
    public Transform TransitionSpot;
    void Start()
    {
        Item = GetComponentInChildren<Pickupable>();
        Collider = GetComponent<Collider>();
    }

    void OnMouseDown()
    {
        if (PlayerController.Instance.HeldItem != null && Item == null)
            StartCoroutine(TakeItem());
        else if (PlayerController.Instance.HeldItem == null && Item != null)
            StartCoroutine(GiveItem());
    }

    IEnumerator GiveItem()
    {
        PlayerController.Instance.HeldItem = Item;
        Item = null;
        if (TransitionSpot != null)
            yield return PlayerController.Instance.HeldItem.PickUp(TransitionSpot);
        yield return PlayerController.Instance.HeldItem.PickUp(PlayerController.Instance.HandTransform);
    }

    IEnumerator TakeItem()
    {
        Item = PlayerController.Instance.HeldItem;
        PlayerController.Instance.HeldItem = null;
        if (TransitionSpot != null)
            yield return Item.PickUp(TransitionSpot);
        yield return Item.PickUp(transform);
    }
}