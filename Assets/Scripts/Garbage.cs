using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : Interactible
{
    public override bool OnInteract(PlayerController player)
    {
        Destroy(player.HeldItem.gameObject);
        player.HeldItem = null;
        return true;
    }
}
