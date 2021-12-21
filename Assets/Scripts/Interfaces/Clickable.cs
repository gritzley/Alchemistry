using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    /// <summary>
    /// On Click bhaviour for this Object
    /// </summary>
    /// <param name="player">Reference to the Player who clicked the item</param>
    public abstract void OnClick(PlayerController player);

}
