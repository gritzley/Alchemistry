using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour, IClickable
{
    [System.Serializable]
    public struct Conversion
    {
        public Ingredient Input;
        public Ingredient Output;
    }

    [SerializeField] private List<Conversion> Conversions;
    [SerializeField] private GameObject IngredientPrefab;

    public IngredientContainer IngredientContainer;
    
    public void ConvertIngredinet()
    {
        foreach (Conversion conversion in Conversions)
        {
            if (conversion.Input == IngredientContainer.Ingredient)
            {
                Transform handTransform = IngredientContainer.gameObject.transform.parent;
                GameObject newIngredient = Object.Instantiate(IngredientPrefab, handTransform.position, Quaternion.identity);
                Object.Destroy(IngredientContainer.gameObject);
                IngredientContainer = newIngredient.GetComponent<IngredientContainer>();
                Debug.Log("No you aren't");
                IngredientContainer.Ingredient = conversion.Output;
                IngredientContainer.gameObject.transform.parent = handTransform;
                IngredientContainer.gameObject.transform.rotation = handTransform.rotation;
                return;
            }
        }
    }

    public void OnClick(PlayerController player)
    {
        if (player.HeldItem != null && player.HeldItem is IngredientContainer)
        {
            IngredientContainer = (IngredientContainer)player.HeldItem;
            ConvertIngredinet();
            player.HeldItem = (Pickupable)IngredientContainer;
        }
    }

}
