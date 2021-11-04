using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    // The name of the ingredient
    public string Name;
    // If this is true, putting this in a kettle will destroy the gameobject
    public bool DestroyOnUse;
}