using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : Interactible
{
    public Transform receiverPoint;
    private Stack<Pickupable> Items;
    void OnEnable() 
    {
        Items = new Stack<Pickupable>();
    }
    public override bool OnInteract(PlayerController player)
    {
        // ---- THROW IN TRASH ----
        if (player.HeldItem != null) 
        {
            new Task( player.HeldItem.MoveTowards(receiverPoint.position, player.HeldItem.animationTime / 3 * 2) )
            .Finished += delegate 
            {
                new Task(player.HeldItem.MoveTowards(transform.position, player.HeldItem.animationTime / 3))
                .Finished += delegate 
                {
                    Items.Push(player.HeldItem);
                    player.HeldItem.transform.SetParent(transform);
                    player.HeldItem = null;
                };
            };
        }

        // ---- TAKE OUT OF TRASH ----
        if (player.HeldItem == null && Items.Count > 0) 
        {
            player.HeldItem = Items.Pop();
            new Task( player.HeldItem.MoveTowards(receiverPoint.position, player.HeldItem.animationTime / 3 * 2) )
            .Finished += delegate 
            {
                new Task(player.HeldItem.MoveTowards(player.HandTransform.position, player.HeldItem.animationTime / 3))
                .Finished += delegate 
                {
                    player.HeldItem.transform.SetParent(player.HandTransform);
                };
            };
        }
        return true;
    }
}
