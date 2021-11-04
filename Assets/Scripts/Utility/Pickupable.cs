using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Moveable, IClickable
{
    // The initial rotation of the object
    [HideInInspector] public Quaternion Rotation;

    void Start()
    {
        // Save initial rotation
        Rotation = transform.rotation;
    }

}
