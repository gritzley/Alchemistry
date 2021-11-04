using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    // The item stored in this spot
    [HideInInspector] public Pickupable Item;


    // Called before first frame
    void Start() {
        // Get item and particlesystem
        Item = GetComponentInChildren<Pickupable>();
    }
}