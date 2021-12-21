using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Clickable
{
    [System.Serializable]
    private struct Conversion
    {
        public Ingredient.Type Input;
        public GameObject Output;
    }

    [SerializeField] private List<Conversion> Conversions;
    
    public GameObject GetConvertedIngredientPrefab(Ingredient.Type type)
    {
        foreach (Conversion conversion in Conversions)
        {
            if (conversion.Input == type)
            {
                return conversion.Output;
            }
        }
        return null;
    }

    public override void OnClick(PlayerController player)
    {
        if (player.HeldItem != null && player.HeldItem is Ingredient)
        {
            GameObject prefab = GetConvertedIngredientPrefab((player.HeldItem as Ingredient).type);
            GameObject ingredient = Object.Instantiate(prefab, player.HandTransform.position, Quaternion.identity);
            Object.Destroy(player.HeldItem.gameObject);
            player.HeldItem = ingredient.GetComponent<Ingredient>();
            ingredient.transform.parent = player.HandTransform;
            ingredient.transform.rotation = player.HandTransform.rotation;
        }
    }
}
