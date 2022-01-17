using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngredientDispenser : Interactible
{
    public bool IsEndless;
    public List<GameObject> IngredientModels;
    private List<GameObject> currentlyHeldIngredients;
    private GameObject randomIngredient => currentlyHeldIngredients[new System.Random().Next(currentlyHeldIngredients.Count)];
    private bool hasIngredients => currentlyHeldIngredients.Count > 0;
    
    private void OnEnable()
    {
        currentlyHeldIngredients = IngredientModels;
    }
    public override bool OnInteract(PlayerController player)
    {
        if (player.HeldItem == null && hasIngredients)
        {
            GameObject go = UnityEngine.Object.Instantiate(GameManager.Instance.IngredientPrefab);
            Ingredient ingredient = go.GetComponent<Ingredient>();
            IngredientDefinition ingredientDefinition = ScriptableObject.CreateInstance<IngredientDefinition>();
            ingredientDefinition.IsConsumable = true;
            GameObject harvest = randomIngredient;
            ingredientDefinition.Model = harvest;
            ingredient.Definition = ingredientDefinition;
            ingredient.transform.parent = player.HandTransform;
            ingredient.transform.position = player.HandTransform.position;
            ingredient.AttachModel();
            player.HeldItem = ingredient;
            if (!IsEndless)
            {
                harvest.SetActive(false);
                currentlyHeldIngredients.Remove(harvest);
            }
        }
        return true;
    }
}
