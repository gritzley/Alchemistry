using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient Definiton", menuName = "Ingredient Definiton")]
public class IngredientDefinition : ScriptableObject
{
    public bool IsConsumable;
    public bool IsStackable;
}
