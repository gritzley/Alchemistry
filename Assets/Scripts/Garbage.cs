using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    public Transform receiverPoint;
    private Stack<Pickupable> Items;
    void OnEnable() 
    {
        Items = new Stack<Pickupable>();
        foreach (Pickupable item in GetComponentsInChildren<Pickupable>())
            Items.Push(item);
    }
    public void OnMouseDown()
    {
        PlayerController player = PlayerController.Instance;
        
        if (player.HeldItem != null) new Task(TakeItem());
        else if (player.HeldItem == null) new Task(ReturnItem());
    }

    private IEnumerator TakeItem()
    {
        PlayerController player = PlayerController.Instance;

        yield return player.HeldItem.PickUp(receiverPoint);
        yield return player.HeldItem.PickUp(transform);

        Items.Push(player.HeldItem);
        player.HeldItem = null;
    }

    private IEnumerator ReturnItem()
    {
        if (Items.Count > 0)
        {
            PlayerController player = PlayerController.Instance;

            player.HeldItem = Items.Pop();

            yield return player.HeldItem.PickUp(receiverPoint);
            yield return player.HeldItem.PickUp(player.HandTransform);
        }
    }
}
