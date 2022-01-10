using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    /// <summary>
    /// On Click bhaviour for this Object
    /// </summary>
    /// <param name="player">Reference to the Player who clicked the item</param>
    /// <return>Return true if the interact event is being used up</return>
    public abstract bool OnInteract(PlayerController player);
}
