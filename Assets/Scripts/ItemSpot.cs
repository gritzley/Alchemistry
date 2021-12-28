using System.Collections;
using UnityEngine;

public class ItemSpot : Clickable
{
    // The item stored in this spot
    [SerializeField] private Pickupable Item;

    // Called before first frame
    void Start()
    {
        // Initialize the Item to the contained Ingredient at the beginning of the game
        Item = GetComponentInChildren<Pickupable>();
    }

    public override void OnClick(PlayerController player)
    {
        // Pick up items
        if (player.HeldItem == null && Item != null)
        {            
            player.HeldItem = Item;
            // Attach item to own transform 
            Item.transform.parent = player.HandTransform;

            // disable input to prevent glitches while taking an item in hand
            player.InAction = true;

            // Move item to position and rotation of handTransform
            StartCoroutine(Item.MoveTowards(player.HandTransform.position));
            StartCoroutine(Item.TurnTowards(player.HandTransform.rotation));

            // Move Item reference from Spot to player
            Item = null;

            // C# does not allow yield return in annonymus functions so we define a new coroutine to reenambe input
            // after item animation ends.
            IEnumerator coroutine () {
                yield return new WaitForSeconds(player.HeldItem.animationTime);
                player.InAction = false;
            }
            StartCoroutine(coroutine());
        }

        // Place Items on Spot
        else if (player.HeldItem != null && Item == null)
        {
            // Set spots item to currently held item
            Item = player.HeldItem;
            // Set item parent to spot
            Item.transform.parent = transform;

            // Move item to the spots location with it's original rotation
            StartCoroutine(Item.MoveTowards(transform.position));
            StartCoroutine(Item.TurnTowards(Item.Rotation));

            // Finally set the players held item to null
            player.HeldItem = null;
        }
    }

    public void OnHover(PlayerController player) {}
}