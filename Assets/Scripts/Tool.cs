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
        public IngredientDefinition Output;
    }

    [SerializeField] private List<Conversion> Conversions;

    public void OnMouseDown()
    {
        PlayerController player = PlayerController.Instance;
        if (player.HeldItem != null && player.HeldItem is Ingredient)
        {
            IngredientDefinition definition = Conversions.Find(e => e.Input == player.HeldItem).Output;
            
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
