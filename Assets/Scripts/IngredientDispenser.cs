using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngredientDispenser : Clickable
{
    public bool IsEndless;
    public List<GameObject> IngredientModels;
    public IngredientDefinition Definition;
    private List<GameObject> currentlyHeldIngredients;
    private GameObject randomIngredient => currentlyHeldIngredients[new System.Random().Next(currentlyHeldIngredients.Count)];
    private bool hasIngredients => currentlyHeldIngredients.Count > 0;
    
    private void OnEnable()
    {
        currentlyHeldIngredients = IngredientModels;
    }
    public override void OnClick()
    {
        PlayerController player = PlayerController.Instance;
        if (player.HeldItem == null && hasIngredients)
        {
            GameObject go = UnityEngine.Object.Instantiate(GameManager.Instance.IngredientPrefab);
            Ingredient ingredient = go.GetComponent<Ingredient>();
            GameObject harvest = randomIngredient;
            Definition.Model = harvest;
            ingredient.Definition = Definition;
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
    }
}
