using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Moveable
{
    // The initial rotation of the object
    [HideInInspector] public Quaternion Rotation;

    void OnEnable()
    {
        // Save initial rotation
        Rotation = transform.rotation;
    }
    
}
