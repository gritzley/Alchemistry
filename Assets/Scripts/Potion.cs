using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Pickupable
{
    public PotionDefinition Definition;

    void OnEnable()
    {
        GetComponentInChildren<TextDisplay>()?.DisplayText(Definition.name);
    }
}
