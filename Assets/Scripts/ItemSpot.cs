using System.Collections;
using UnityEngine;

public class ItemSpot : Interactible
{
    private Pickupable Item;

    void Start()
    {
        Item = GetComponentInChildren<Pickupable>();
        SetChildInteractibleParent();
    }

    public override bool OnInteract(PlayerController player)
    {
        // REMOVE FROM SPOT
        if (player.HeldItem == null && Item != null)
        {
            player.HeldItem = Item;
            new Task( MoveItem(player.HandTransform, player) )
            .Finished += delegate{
                Item = null;
                Destroy(player.HeldItem.GetComponentInChildren<InteractibleChild>());
            };

            return true;
        }

        // PLACE IN SPOT
        else if (player.HeldItem != null && Item == null)
        {
            Item = player.HeldItem;
            new Task( MoveItem(transform, player) )
            .Finished += delegate{
                player.HeldItem = null;
                SetChildInteractibleParent();
            };

            return true;
        }

        return false;
    }

    IEnumerator MoveItem (Transform target, PlayerController player)
    {
        player.InAction = true;
        Item.transform.parent = target;

        Task Moving = new Task(Item.MoveTowards(target.position));
        Task Turning = new Task(Item.TurnTowards(target.rotation));

        while (Moving.Running && Turning.Running) yield return new WaitForSeconds(0.1f);
        player.InAction = false;
    }

    void SetChildInteractibleParent()
    {
        InteractibleChild child = Item?.GetComponentInChildren<MeshRenderer>()?.gameObject.AddComponent<InteractibleChild>();
        if (child != null)
        {
            child.Parent = this;
        }
    }
}