using UnityEngine;

public class InteractibleChild : Interactible
{
    public Interactible Parent;
    public override bool OnInteract(PlayerController player)
    {
        return (bool)Parent?.OnInteract(player);
    }
}
