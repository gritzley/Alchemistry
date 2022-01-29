using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public GameObject IngredientPrefab;
    [System.Serializable]
    private struct Conversion
    {
        public IngredientDefinition Input;
        public Ingredient Output;
    }
    [SerializeField] private List<Conversion> Conversions;

    public void OnMouseDown()
    {
        PlayerController player = PlayerController.Instance;
        if (player.HeldItem != null && player.HeldItem is Ingredient)
        {
            Ingredient prefab = Conversions.Find(e => e.Input == player.HeldItem).Output;

            GameObject go = Instantiate(prefab.gameObject);
            Ingredient ingredient = go.GetComponent<Ingredient>();
            
            player.HeldItem = ingredient;
            ingredient.PickUp(player.HandTransform);
        }
    }
}
