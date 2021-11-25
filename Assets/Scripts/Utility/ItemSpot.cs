using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    // The item stored in this spot
    [HideInInspector] public Pickupable Item;

    // Called before first frame
    void Start()
    {
        // Initialize the Item at the beginning of the game.
        Item = GetComponentInChildren<Pickupable>();
    }
}