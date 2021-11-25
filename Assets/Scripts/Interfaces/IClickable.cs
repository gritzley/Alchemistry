using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    /// <summary>
    /// On Click bhaviour for this Object
    /// </summary>
    /// <param name="player">Reference to the Player who clicked the item</param>
    void OnClick(PlayerController player);
}
