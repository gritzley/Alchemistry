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
            Item.transform.parent = player.HandTransform;

            player.InAction = true;

            StartCoroutine(Item.MoveTowards(player.HandTransform.position));
            StartCoroutine(Item.TurnTowards(player.HandTransform.rotation));

            Item = null;

            // C# does not allow yield return in annonymus functions so we define a new coroutine to reenablee input
            // after item animation ends.
            IEnumerator coroutine () {
                yield return new WaitForSeconds(player.HeldItem.animationTime);
                player.InAction = false;
            }
            StartCoroutine(coroutine());

            Destroy(player.HeldItem.GetComponentInChildren<InteractibleChild>());

            return true;
        }

        // PLACE IN SPOT
        else if (player.HeldItem != null && Item == null)
        {
            Item = player.HeldItem;
            Item.transform.parent = transform;

            StartCoroutine(Item.MoveTowards(transform.position));
            StartCoroutine(Item.TurnTowards(Item.Rotation));

            player.HeldItem = null;

            SetChildInteractibleParent();
            return true;
        }
        return false;
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