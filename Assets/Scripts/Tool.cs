using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Clickable
{
    public GameObject IngredientPrefab;
    [System.Serializable]
    private struct Conversion
    {
        public IngredientDefinition Input;
        public IngredientDefinition Output;
    }

    [SerializeField] private List<Conversion> Conversions;
    
    public IngredientDefinition GetConvertedIngredientPrefab(IngredientDefinition input)
    {
        foreach (Conversion conversion in Conversions)
        {
            if (conversion.Input == input)
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
            IngredientDefinition definition = GetConvertedIngredientPrefab((player.HeldItem as Ingredient).Definition);
            
            GameObject go = Object.Instantiate(IngredientPrefab);
            Object.Destroy(player.HeldItem.gameObject);
            player.HeldItem = go.GetComponent<Ingredient>();

            Ingredient ingredient = go.GetComponent<Ingredient>();
            ingredient.Definition = definition;
            ingredient.AttachModel();

            go.transform.parent = player.HandTransform;
            go.transform.position = player.HandTransform.transform.position;
            go.transform.rotation = player.HandTransform.rotation;
        }
    }
}
